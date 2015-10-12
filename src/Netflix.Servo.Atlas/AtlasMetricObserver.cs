using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web;
using Java.Util.Concurrent;
using Netflix.Servo.Attributes;
using Netflix.Servo.Monitor;
using Netflix.Servo.Publish;
using Netflix.Servo.Tag;
using Netflix.Servo.Util;
using slf4net;

namespace Netflix.Servo.Atlas
{
    /**
 * Observer that forwards metrics to atlas. In addition to being MetricObserver, it also supports
 * a push model that sends metrics as soon as possible (asynchronously).
 */
    public class AtlasMetricObserver : MetricObserver
    {
        private static ILogger LOGGER = LoggerFactory.GetLogger(typeof(AtlasMetricObserver));
        private static ITag ATLAS_COUNTER_TAG = new BasicTag("atlas.dstype", "counter");
        private static ITag ATLAS_GAUGE_TAG = new BasicTag("atlas.dstype", "gauge");
        private static UpdateTasks NO_TASKS = new UpdateTasks(0, null, -1L);
        protected HttpHelper httpHelper;
        protected ServoAtlasConfig config;
        protected long sendTimeoutMs; // in milliseconds
        protected long stepMs; // in milliseconds
        private Counter numMetricsTotal = Monitors.newCounter("numMetricsTotal");
        private Timer updateTimer = Monitors.newTimer("update");
        private Counter numMetricsDroppedSendTimeout = newErrCounter("numMetricsDropped",
            "sendTimeout");
        private Counter numMetricsDroppedQueueFull = newErrCounter("numMetricsDropped",
            "sendQueueFull");
        private Counter numMetricsDroppedHttpErr = newErrCounter("numMetricsDropped",
            "httpError");
        private Counter numMetricsSent = Monitors.newCounter("numMetricsSent");
        private TagList commonTags;
        public static LinkedBlockingQueue<UpdateTasks> pushQueue = new LinkedBlockingQueue<UpdateTasks>();
        //@SuppressWarnings("unused")
        private Gauge<int> pushQueueSize = new BasicGauge<int>(
            MonitorConfig.builder("pushQueue").build(), new Callable<int>(() => pushQueue.Count));

        /**
         * Create an observer that can send metrics to atlas with a given
         * config and list of common tags.
         * This method will use the default poller index of 0.
         */
        public AtlasMetricObserver(ServoAtlasConfig config, TagList commonTags)
              : this(config, commonTags, 0)
        {
        }

        /**
         * Create an observer that can send metrics to atlas with a given config, list of common tags,
         * and poller index.
         */
        public AtlasMetricObserver(ServoAtlasConfig config, TagList commonTags, int pollerIdx)
            : this(config, commonTags, pollerIdx, new HttpHelper(new RxHttp(new BasicServerRegistry())))
        {

        }

        /**
         * Create an atlas observer. For internal use of servo only.
         */
        public AtlasMetricObserver(ServoAtlasConfig config,
                                   TagList commonTags,
                                   int pollerIdx,
                                   HttpHelper httpHelper)
        {
            this.httpHelper = httpHelper;
            this.config = config;
            this.stepMs = Pollers.getPollingIntervals().ElementAt(pollerIdx);
            this.sendTimeoutMs = stepMs * 9 / 10;
            this.commonTags = commonTags;
            pushQueue = new LinkedBlockingQueue<UpdateTasks>(config.getPushQueueSize());
            Thread pushThread = new Thread(new PushProcessor(), "BaseAtlasMetricObserver-Push");
            pushThread.setDaemon(true);
            pushThread.Start();
        }

        protected static Counter newErrCounter(String name, String err)
        {
            return new BasicCounter(MonitorConfig.builder(name).withTag("error", err).build());
        }

        protected static Metric asGauge(Metric m)
        {
            return new Metric(m.getConfig().withAdditionalTag(ATLAS_GAUGE_TAG),
                m.getTimestamp(), m.getValue());
        }

        protected static Metric asCounter(Metric m)
        {
            return new Metric(m.getConfig().withAdditionalTag(ATLAS_COUNTER_TAG),
                m.getTimestamp(), m.getValue());
        }

        protected static bool isCounter(Metric m)
        {
            TagList tags = m.getConfig().getTags();
            String value = tags.getValue(DataSourceType.KEY);
            return value != null && value.Equals(DataSourceType.COUNTER.name);
        }

        protected static bool isGauge(Metric m)
        {
            TagList tags = m.getConfig().getTags();
            String value = tags.getValue(DataSourceType.KEY);
            return value != null && value.Equals(DataSourceType.GAUGE.name);
        }

        protected static bool isRate(Metric m)
        {
            TagList tags = m.getConfig().getTags();
            String value = tags.getValue(DataSourceType.KEY);
            return DataSourceType.RATE.name.Equals(value)
                || DataSourceType.NORMALIZED.name.Equals(value);
        }

        protected static List<Metric> identifyDsTypes(List<Metric> metrics)
        {
            // since we never generate atlas.dstype = counter we can do the following:

            return metrics.Select(m => isRate(m) ? m : asGauge(m)).ToList();

            //return metrics.stream().map(m->isRate(m) ? m : asGauge(m)).collect(Collectors.toList());
        }

        public String getName()
        {
            return "atlas";
        }

        private List<Metric> identifyCountersForPush(List<Metric> metrics)
        {
            List<Metric> transformed = new List<Metric>();
            foreach (var m in metrics)
            {
                Metric toAdd = m;
                if (isCounter(m))
                {
                    toAdd = asCounter(m);
                }
                else if (isGauge(m))
                {
                    toAdd = asGauge(m);
                }
                transformed.Add(toAdd);
            }
            return transformed;
        }

        /**
         * Immediately send metrics to the backend.
         *
         * @param rawMetrics Metrics to be sent. Names and tags will be sanitized.
         */
        public void push(List<Metric> rawMetrics)
        {
            List<Metric> validMetrics = ValidCharacters.toValidValues(filter(rawMetrics));
            List<Metric> metrics = transformMetrics(validMetrics);

            LOGGER.Debug("Scheduling push of {} metrics", metrics.Count);
            UpdateTasks tasks = getUpdateTasks(BasicTagList.EMPTY,
                identifyCountersForPush(metrics));
            int maxAttempts = 5;
            int attempts = 1;
            while (!pushQueue.Offer(tasks) && attempts <= maxAttempts)
            {
                ++attempts;
                UpdateTasks droppedTasks = pushQueue.Remove();
                LOGGER.Warn("Removing old push task due to queue full. Dropping {} metrics.",
                    droppedTasks.numMetrics);
                numMetricsDroppedQueueFull.increment(droppedTasks.numMetrics);
            }
            if (attempts >= maxAttempts)
            {
                LOGGER.Error("Unable to push update of {}", tasks);
                numMetricsDroppedQueueFull.increment(tasks.numMetrics);
            }
            else
            {
                LOGGER.Debug("Queued push of {}", tasks);
            }
        }

        protected void sendNow(UpdateTasks updateTasks)
        {
            if (updateTasks.numMetrics == 0)
            {
                return;
            }

            Stopwatch s = updateTimer.start();
            int totalSent = 0;
            try
            {
                totalSent = httpHelper.sendAll(updateTasks.tasks,
                    updateTasks.numMetrics, sendTimeoutMs);
                LOGGER.Debug("Sent {}/{} metrics to atlas", totalSent, updateTasks.numMetrics);
            }
            finally
            {
                s.stop();
                int dropped = updateTasks.numMetrics - totalSent;
                numMetricsDroppedSendTimeout.increment(dropped);
            }
        }

        protected bool shouldIncludeMetric(Metric metric)
        {
            return true;
        }

        /**
         * Return metrics to be sent to the main atlas deployment.
         * Metrics will be sent if their publishing policy matches atlas and if they
         * will *not* be sent to the aggregation cluster.
         */
        protected List<Metric> filter(List<Metric> metrics)
        {
            List<Metric> filtered = metrics.Where(x => shouldIncludeMetric(x)).ToList();
            LOGGER.Debug("Filter: input {} metrics, output {} metrics",
                metrics.Count, filtered.Count);
            return filtered;
        }

        protected List<Metric> transformMetrics(List<Metric> metrics)
        {
            return metrics;
        }

        public void update(List<Metric> rawMetrics)
        {
            List<Metric> valid = ValidCharacters.toValidValues(rawMetrics);
            List<Metric> metrics = identifyDsTypes(filter(valid));
            List<Metric> transformed = transformMetrics(metrics);
            sendNow(getUpdateTasks(commonTags, transformed));
        }

        private UpdateTasks getUpdateTasks(TagList tags, List<Metric> metrics)
        {
            if (!config.shouldSendMetrics())
            {
                LOGGER.Debug("Plugin disabled or running on a dev environment. Not sending metrics.");
                return NO_TASKS;
            }

            if (metrics.Count == 0)
            {
                LOGGER.Debug("metrics list is empty, no data being sent to server");
                return NO_TASKS;
            }

            int numMetrics = metrics.Count;
            Metric[] atlasMetrics = new Metric[metrics.Count];
            atlasMetrics = metrics.ToArray();

            numMetricsTotal.increment(numMetrics);
            List<ObservableCollection<int>> tasks = new List<ObservableCollection<int>>();
            String uri = config.getAtlasUri();
            LOGGER.Debug("writing {} metrics to atlas ({})", numMetrics, uri);
            int i = 0;
            while (i < numMetrics)
            {
                int remaining = numMetrics - i;
                int batchSize = Math.Min(remaining, config.batchSize());
                Metric[] batch = new Metric[batchSize];
                batch = atlasMetrics.ToArray();
                ObservableCollection<int> sender = getSenderObservable(tags, batch);
                tasks.Add(sender);
                i += batchSize;
            }
            System.Diagnostics.Debug.Assert(i == numMetrics);
            LOGGER.Debug("succeeded in creating {} observable(s) to send metrics with total size {}",
                tasks.Count, numMetrics);

            return new UpdateTasks(numMetrics * getNumberOfCopies(), tasks,
                ClockWithOffset.INSTANCE.WALL());
        }

        protected int getNumberOfCopies()
        {
            return 1;
        }

        protected ObservableCollection<int> getSenderObservable(TagList tags, Metric[] batch)
        {
            JsonPayload payload = new UpdateRequest(tags, batch, batch.Length);
            return httpHelper.postSmile(config.getAtlasUri(), payload)
                .map(withBookkeeping(batch.Length));
        }

        /**
         * Utility function to map an Observable&lt;ByteBuf> to an Observable&lt;Integer> while also
         * updating our counters for metrics sent and errors.
         */
        protected Func<HttpResponse, int> withBookkeeping(int batchSize)
        {
            return response =>
            {
                bool ok = response.StatusCode == 200;
                if (ok)
                {
                    numMetricsSent.increment(batchSize);
                }
                else
                {
                    LOGGER.Info("Status code: {} - Lost {} metrics",
                        response.StatusCode, batchSize);
                    numMetricsDroppedHttpErr.increment(batchSize);
                }

                return batchSize;
            };
        }

        public class UpdateTasks
        {
            public int numMetrics;
            public List<ObservableCollection<int>> tasks;
            private long timestamp;

            public UpdateTasks(int numMetrics, List<ObservableCollection<int>> tasks, long timestamp)
            {
                this.numMetrics = numMetrics;
                this.tasks = tasks;
                this.timestamp = timestamp;
            }


            public override String ToString()
            {
                return "UpdateTasks{numMetrics=" + numMetrics + ", tasks="
                    + tasks + ", timestamp=" + timestamp + '}';
            }
        }

        private class PushProcessor : IRunnable
        {
            public void Run()
            {
                bool interrupted = false;
                while (!interrupted)
                {
                    try
                    {
                        sendNow(pushQueue.Take());
                    }
                    catch (CancellationException e)
                    {
                        LOGGER.Debug("Interrupted trying to get next UpdateTask to push");
                        interrupted = true;
                    }
                    catch (Exception t)
                    {
                        LOGGER.Info("Caught unexpected exception pushing metrics", t);
                    }
                }
            }
        }
    }
}
