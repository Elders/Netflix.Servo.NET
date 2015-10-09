using System.Collections.Generic;

namespace Netflix.Servo.Publish
{
    /**
 * A poller that can be used to fetch current values for a list of metrics on
 * demand.
 */
    public interface MetricPoller
    {
        /**
         * Fetch the current values for a set of metrics that match the provided
         * filter. This method should be cheap, thread-safe, and interruptible so
         * that it can be called frequently to collect metrics at a regular
         * interval.
         *
         * @param filter retricts the set of metrics
         * @return list of current metric values
         */
        IEnumerable<Metric> poll(MetricFilter filter);

        /**
         * Fetch the current values for a set of metrics that match the provided
         * filter. This method should be cheap, thread-safe, and interruptible so
         * that it can be called frequently to collect metrics at a regular
         * interval.
         *
         * @param filter retricts the set of metrics
         * @param reset  ignored. This is kept for backwards compatibility only.
         * @return list of current metric values
         */
        IEnumerable<Metric> poll(MetricFilter filter, bool reset);
    }
}
