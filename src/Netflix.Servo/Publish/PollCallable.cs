using System.Collections.Generic;
using System.Linq;
using Java.Util.Concurrent;

namespace Netflix.Servo.Publish
{
    /**
 * Callable implementation that invokes the {@link MetricPoller#poll} method.
 */
    public class PollCallable : Callable<List<Metric>>
    {

        private MetricPoller poller;
        private MetricFilter filter;
        private bool reset;

        /**
         * Creates a new instance.
         *
         * @param poller poller to invoke
         * @param filter filter to pass into the poller
         */
        public PollCallable(MetricPoller poller, MetricFilter filter)
            : this(poller, filter, false)
        {

        }

        /**
         * Creates a new instance.
         *
         * @param poller poller to invoke
         * @param filter filter to pass into the poller
         * @param reset  reset flag to pass into the poller
         */
        public PollCallable(MetricPoller poller, MetricFilter filter, bool reset)
            : base(() => poller.poll(filter, reset).ToList())
        {
            this.poller = poller;
            this.filter = filter;
            this.reset = reset;
        }

        /**
         * {@inheritDoc}
         */
        public List<Metric> call()
        {
            return poller.poll(filter, reset).ToList();
        }
    }
}
