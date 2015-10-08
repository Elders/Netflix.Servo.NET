using Netflix.Servo.Util;

namespace Netflix.Servo.Monitor
{
    /// <summary>
    ///  Base type to simplify implementing monitors.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class AbstractMonitor<T> : Monitor<T>
    {
        protected readonly MonitorConfig config;

        /// <summary>
        /// Create a new instance with the specified configuration.
        /// </summary>
        /// <param name="config"></param>
        protected AbstractMonitor(MonitorConfig config)
        {
            this.config = Preconditions.checkNotNull(config, "config");
        }

        public MonitorConfig getConfig()
        {
            return config;
        }

        public T getValue()
        {
            return getValue(0);
        }

        public abstract T getValue(int pollerIndex);
    }
}
