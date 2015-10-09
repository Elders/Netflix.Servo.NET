using System;
using System.Collections.Generic;
using Netflix.Servo.Attributes;
using Netflix.Servo.Monitor;
using Netflix.Servo.Tag;
using Netflix.Servo.Util;
using slf4net;

namespace Netflix.Servo.Publish
{
    /**
 * Converts counter metrics into a rate per second. The rate is calculated by
 * comparing two samples of given metric and looking at the delta. Since two
 * samples are needed to calculate the rate, no value will be sent to the
 * wrapped observer until a second sample arrives. If a given metric is not
 * updated within a given heartbeat interval, the previous cached value for the
 * counter will be dropped such that if a new sample comes in it will be
 * treated as the first sample for that metric.
 * <p/>
 * <p>Counters should be monotonically increasing values. If a counter value
 * decreases from one sample to the next, then we will assume the counter value
 * was reset and send a rate of 0. This is similar to the RRD concept of
 * type DERIVE with a min of 0.
 * <p/>
 * <p>This class is not thread safe and should generally be wrapped by an async
 * observer to prevent issues.
 */
    public class CounterToRateMetricTransform : MetricObserver
    {

        private static ILogger LOGGER = LoggerFactory.GetLogger(typeof(CounterToRateMetricTransform));

        private static String COUNTER_VALUE = DataSourceType.COUNTER.name;
        private static ITag RATE_TAG = DataSourceType.RATE;

        private MetricObserver observer;
        //private Dictionary<MonitorConfig, CounterValue> cache;

        private long intervalMillis;

        /**
         * Creates a new instance with the specified heartbeat interval. The
         * heartbeat should be some multiple of the sampling interval used when
         * collecting the metrics.
         */
        public CounterToRateMetricTransform(MetricObserver observer, TimeSpan heartbeat)
                : this(observer, heartbeat, TimeSpan.FromMilliseconds(0))
        {

        }

        /**
         * Creates a new instance with the specified heartbeat interval. The
         * heartbeat should be some multiple of the sampling interval used when
         * collecting the metrics.
         *
         * @param observer           downstream observer to forward values to after the rate has
         *                           been computed.
         * @param heartbeat          how long to remember a previous value before dropping it and
         *                           treating new samples as the first report.
         * @param estPollingInterval estimated polling interval in to use for the first call. If set
         *                           to zero no values will be forwarded until the second sample for
         *                           a given counter. The delta for the first interval will be the
         *                           total value for the counter as it is assumed it started at 0 and
         *                           was first created since the last polling interval. If this
         *                           assumption is not true then this setting should be 0 so it waits
         *                           for the next sample to compute an accurate delta, otherwise
         *                           spikes will occur in the output.
         * @param unit               unit for the heartbeat and estPollingInterval params.
         * @param clock              Clock instance to use for getting the time.
         */

        //TODO: This original with cache
        //        CounterToRateMetricTransform(MetricObserver observer, TimeSpan heartbeat, TimeSpan estPollingInterval, Clock clock)
        //        {
        //            this.observer = observer;
        //            this.intervalMillis = TimeUnit.MILLISECONDS.convert(estPollingInterval, unit);

        //            long heartbeatMillis = TimeUnit.MILLISECONDS.convert(heartbeat, unit);
        //            this.cache = new Dictionary<MonitorConfig, CounterValue>(16, 0.75f, true);
        //            {
        //                              protected bool removeEldestEntry(Map.Entry<MonitorConfig, CounterValue> eldest)
        //        {
        //            long now = clock.now();
        //            long lastMod = eldest.getValue().getTimestamp();
        //            bool expired = (now - lastMod) > heartbeatMillis;
        //            if (expired)
        //            {
        //                LOGGER.Debug("heartbeat interval exceeded, expiring {}", eldest.getKey());
        //            }
        //            return expired;
        //        }
        //    };
        //}


        CounterToRateMetricTransform(MetricObserver observer, TimeSpan heartbeat, TimeSpan estPollingInterval)
        {
            this.observer = observer;
            this.intervalMillis = (long)estPollingInterval.TotalMilliseconds;

            long heartbeatMillis = (long)heartbeat.TotalMilliseconds;
        }

        ///**
        // * Creates a new instance with the specified heartbeat interval. The
        // * heartbeat should be some multiple of the sampling interval used when
        // * collecting the metrics.
        // *
        // * @param observer           downstream observer to forward values to after the rate has
        // *                           been computed.
        // * @param heartbeat          how long to remember a previous value before dropping it and
        // *                           treating new samples as the first report.
        // * @param estPollingInterval estimated polling interval in to use for the first call. If set
        // *                           to zero no values will be forwarded until the second sample for
        // *                           a given counter. The delta for the first interval will be the
        // *                           total value for the counter as it is assumed it started at 0 and
        // *                           was first created since the last polling interval. If this
        // *                           assumption is not true then this setting should be 0 so it waits
        // *                           for the next sample to compute an accurate delta, otherwise
        // *                           spikes will occur in the output.
        // * @param unit               unit for the heartbeat and estPollingInterval params.
        // */
        //public CounterToRateMetricTransform(MetricObserver observer, TimeSpan heartbeat, TimeSpan estPollingInterval)
        //                            : this(observer, heartbeat, estPollingInterval)
        //{

        //}

        /**
         * {@inheritDoc}
         */
        public String getName()
        {
            return observer.getName();
        }

        /**
         * {@inheritDoc}
         */
        //TODO: This original with cache
        //public void update(List<Metric> metrics)
        //{
        //    Preconditions.checkNotNull(metrics, "metrics");
        //    LOGGER.Debug("received {} metrics", metrics.Count);
        //    List<Metric> newMetrics = new List<Metric>();
        //    foreach (var m in metrics)
        //    {
        //        if (isCounter(m))
        //        {
        //            MonitorConfig rateConfig = toRateConfig(m.getConfig());
        //            CounterValue prev;
        //            if (cache.TryGetValue(rateConfig, out prev))
        //            {
        //                double rate = prev.computeRate(m);
        //                newMetrics.Add(new Metric(rateConfig, m.getTimestamp(), rate));
        //            }
        //            else
        //            {
        //                CounterValue current = new CounterValue(m);
        //                cache.Add(rateConfig, current);
        //                if (intervalMillis > 0L)
        //                {
        //                    double delta = m.getNumberValue().doubleValue();
        //                    double rate = current.computeRate(intervalMillis, delta);
        //                    newMetrics.Add(new Metric(rateConfig, m.getTimestamp(), rate));
        //                }
        //            }
        //        }
        //        else
        //        {
        //            newMetrics.Add(m);
        //        }
        //    }
        //    LOGGER.Debug("writing {} metrics to downstream observer", newMetrics.Count);
        //    observer.update(newMetrics);
        //}

        public void update(List<Metric> metrics)
        {
            Preconditions.checkNotNull(metrics, "metrics");
            LOGGER.Debug("received {} metrics", metrics.Count);
            List<Metric> newMetrics = new List<Metric>();
            foreach (var m in metrics)
            {
                if (isCounter(m))
                {
                    MonitorConfig rateConfig = toRateConfig(m.getConfig());
                    CounterValue current = new CounterValue(m);

                    if (intervalMillis > 0L)
                    {
                        double delta = (double)m.getNumberValue();
                        double rate = current.computeRate(intervalMillis, delta);
                        newMetrics.Add(new Metric(rateConfig, m.getTimestamp(), rate));
                    }
                }
                else
                {
                    newMetrics.Add(m);
                }
            }
            LOGGER.Debug("writing {} metrics to downstream observer", newMetrics.Count);
            observer.update(newMetrics);
        }

        /**
         * Clear all cached state of previous counter values.
         */
        public void reset()
        {
            //cache.Clear();
        }

        /**
         * Convert a MonitorConfig for a counter to one that is explicit about
         * being a RATE.
         */
        private MonitorConfig toRateConfig(MonitorConfig config)
        {
            return config.withAdditionalTag(RATE_TAG);
        }

        private bool isCounter(Metric m)
        {
            TagList tags = m.getConfig().getTags();
            String value = tags.getValue(DataSourceType.KEY);
            return COUNTER_VALUE.Equals(value);
        }

        private class CounterValue
        {
            private long timestamp;
            private double value;

            public CounterValue(long timestamp, double value)
            {
                this.timestamp = timestamp;
                this.value = value;
            }

            public CounterValue(Metric m)
                : this(m.getTimestamp(), (double)m.getNumberValue())
            {

            }

            public long getTimestamp()
            {
                return timestamp;
            }

            public double computeRate(Metric m)
            {
                long currentTimestamp = m.getTimestamp();
                double currentValue = (double)m.getNumberValue();

                long durationMillis = currentTimestamp - timestamp;
                double delta = currentValue - value;

                timestamp = currentTimestamp;
                value = currentValue;

                return computeRate(durationMillis, delta);
            }

            public double computeRate(long durationMillis, double delta)
            {
                double millisPerSecond = 1000.0;
                double duration = durationMillis / millisPerSecond;
                return (duration <= 0.0 || delta <= 0.0) ? 0.0 : delta / duration;
            }
        }
    }
}
