namespace Netflix.Servo.Monitor
{
    /**
 * Provides a way to sample a value tied to a particular configuration.
 */
    public interface Monitor<T>
    {

        /**
         * Returns the current value for the monitor for the default polling interval.
         */
        T getValue();

        /**
         * Returns the current value for the monitor for the nth poller.
         */
        T getValue(int pollerIndex);


        /**
         * Configuration used to identify a monitor and provide metadata used in aggregations.
         */
        MonitorConfig getConfig();
    }
}
