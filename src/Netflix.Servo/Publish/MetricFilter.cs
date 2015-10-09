using Netflix.Servo.Monitor;

namespace Netflix.Servo.Publish
{
    /**
 * A filter to restrict the set of metrics that are polled.
 */
    public interface MetricFilter
    {
        /**
         * Check if a metric with the provided configuration should be selected and
         * sent to observers.
         *
         * @param config config settings associated with the metric
         * @return true if the metric should be selected
         */
        bool matches(MonitorConfig config);
    }
}
