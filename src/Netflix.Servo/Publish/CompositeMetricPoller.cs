using System;
using System.Collections.Generic;
using Java.Util.Concurrent;
using slf4net;

namespace Netflix.Servo.Publish
{
    /**
 * Combines results from a list of metric pollers. This clas
 */
    public class CompositeMetricPoller : MetricPoller
    {

        private static ILogger LOGGER = LoggerFactory.GetLogger(typeof(CompositeMetricPoller));

        private static List<Metric> EMPTY = new List<Metric>();

        private static String POLLER_KEY = "PollerName";

        private Dictionary<String, MetricPoller> pollers;
        private AbstractExecutorService executor;
        private long timeout;

        /**
         * Creates a new instance for a set of pollers.
         *
         * @param pollers  a set of pollers to collect data from, the map key for a
         *                 poller is used as a name identify a particular poller
         *                 for logging and error messages
         * @param executor an executor to use for executing the poll methods
         * @param timeout  timeout in milliseconds used when getting the value
         *                 from the future
         */
        public CompositeMetricPoller(
            Dictionary<String, MetricPoller> pollers, AbstractExecutorService executor, long timeout)
        {
            this.pollers = pollers;
            this.executor = executor;
            this.timeout = timeout;
        }

        private void increment(Exception t, String name)
        {
            //TagList tags = SortedTagList.builder().withTag(new BasicTag(POLLER_KEY, name)).build();
            //Counters.increment(t.getClass().getSimpleName() + "Count", tags);
        }

        private List<Metric> getMetrics(String name, IFuture<List<Metric>> future)
        {
            List<Metric> metrics = EMPTY;
            try
            {
                metrics = future.Get(TimeSpan.FromMilliseconds((double)timeout));
            }
            catch (ExecutionException e)
            {
                increment(e, name);
                LOGGER.Warn("uncaught exception from poll method for " + name, e);
            }
            catch (TimeoutException e)
            {
                // The cancel is needed to prevent the slow task from using up all threads
                future.Cancel(true);
                increment(e, name);
                LOGGER.Warn("timeout executing poll method for " + name, e);
            }
            catch (CancellationException e)
            {
                increment(e, name);
                LOGGER.Warn("interrupted while doing get for " + name, e);
            }
            return metrics;
        }

        /**
         * {@inheritDoc}
         */
        public IEnumerable<Metric> poll(MetricFilter filter)
        {
            return poll(filter, false);
        }

        /**
         * {@inheritDoc}
         */
        public IEnumerable<Metric> poll(MetricFilter filter, bool reset)
        {
            Dictionary<String, IFuture<List<Metric>>> futures = new Dictionary<string, IFuture<List<Metric>>>();

            foreach (var e in pollers)
            {
                PollCallable task = new PollCallable(e.Value, filter, reset);
                futures.Add(e.Key, executor.Submit(task));
            }

            List<Metric> allMetrics = new List<Metric>();
            foreach (var e in futures)
            {
                allMetrics.AddRange(getMetrics(e.Key, e.Value));
            }
            return allMetrics;
        }
    }
}
