namespace Netflix.Servo.Monitor
{
    public interface IMonitor
    {
        object GetValue();

        /**
         * Returns the current value for the monitor for the nth poller.
         */
        object GetValue(int pollerIndex);


        /**
         * Configuration used to identify a monitor and provide metadata used in aggregations.
         */
        MonitorConfig getConfig();
    }

    /**
* Provides a way to sample a value tied to a particular configuration.
*/
    public interface IMonitor<out T> : IMonitor
    {

        /**
         * Returns the current value for the monitor for the default polling interval.
         */
        new T GetValue();

        /**
         * Returns the current value for the monitor for the nth poller.
         */
        new T GetValue(int pollerIndex);


        /**
         * Configuration used to identify a monitor and provide metadata used in aggregations.
         */
        new MonitorConfig getConfig();
    }
}