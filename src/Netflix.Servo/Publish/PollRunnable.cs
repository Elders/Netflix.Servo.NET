using System;
using System.Collections.Generic;
using System.Linq;
using Java.Util.Concurrent;
using Netflix.Servo.Util;
using slf4net;

namespace Netflix.Servo.Publish
{
    /**
 * Runnable that will send updates to a collection of observers.
 */
    public class PollRunnable : IRunnable
    {
        private static ILogger LOGGER =
            LoggerFactory.GetLogger(typeof(PollRunnable));

        private MetricPoller poller;
        private MetricFilter filter;
        private bool reset;
        private IEnumerable<MetricObserver> observers;

        /**
         * Creates a new runnable instance that executes poll with the given filter
         * and sends the metrics to all of the given observers.
         */
        public PollRunnable(
            MetricPoller poller,
            MetricFilter filter,
            IEnumerable<MetricObserver> observers)
          : this(poller, filter, false, observers)
        {
        }

        /**
         * Creates a new runnable instance that executes poll with the given filter
         * and sends the metrics to all of the given observers.
         */
        public PollRunnable(
            MetricPoller poller,
            MetricFilter filter,
            bool reset,
            IEnumerable<MetricObserver> observers)
        {
            this.poller = Preconditions.checkNotNull(poller, "poller");
            this.filter = Preconditions.checkNotNull(filter, "poller");
            this.reset = reset;
            this.observers = observers.ToList().AsReadOnly();
        }

        /**
         * Creates a new runnable instance that executes poll with the given filter
         * and sends the metrics to all of the given observers.
         */
        public PollRunnable(
            MetricPoller poller,
            MetricFilter filter,
            params MetricObserver[] observers)
            : this(poller, filter, false, observers.ToList().AsReadOnly())
        {

        }

        /**
         * {@inheritDoc}
         */
        public void Run()
        {
            try
            {
                IEnumerable<Metric> metrics = poller.poll(filter, reset);
                foreach (var o in observers)
                {
                    try
                    {
                        o.update(metrics.ToList());
                    }
                    catch (Exception t)
                    {
                        LOGGER.Warn("failed to send metrics to " + o.getName(), t);
                    }
                }
            }
            catch (Exception t)
            {
                LOGGER.Warn("failed to poll metrics", t);
            }
        }
    }
}
