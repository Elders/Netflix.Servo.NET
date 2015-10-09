using System.Collections.Generic;
using Netflix.Servo.Util;
using slf4net;
using System.Linq;

namespace Netflix.Servo.Publish
{
    /**
 * Base class for simple pollers that do not benefit from filtering in advance.
 * Sub-classes implement {@link #pollImpl} to return a list and all filtering
 * will be taken care of by the provided implementation of {@link #poll}.
 */
    public abstract class BaseMetricPoller : MetricPoller
    {

        protected ILogger logger = LoggerFactory.GetLogger(typeof(BaseMetricPoller));

        /**
         * Return a list of all current metrics for this poller.
         */
        public abstract List<Metric> pollImpl(bool reset);

        public IEnumerable<Metric> poll(MetricFilter filter)
        {
            return poll(filter, false);
        }

        public IEnumerable<Metric> poll(MetricFilter filter, bool reset)
        {
            Preconditions.checkNotNull(filter, "filter");
            List<Metric> metrics = pollImpl(reset);
            List<Metric> retained = metrics.Where(m => filter.matches(m.getConfig())).ToList();

            logger.Debug("received {} metrics, retained {} metrics", metrics.Count, retained.Count);

            return retained.AsReadOnly();
        }
    }
}
