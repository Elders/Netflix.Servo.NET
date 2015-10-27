namespace Netflix.Servo.Monitor
{
    public abstract class AbstractMonitor<T> : IMonitor<T>
    {
        protected MonitorConfig config;

        /**
         * Create a new instance with the specified configuration.
         */
        protected AbstractMonitor(MonitorConfig config)
        {
            this.config = config;
        }

        public MonitorConfig getConfig()
        {
            return config;
        }

        public virtual T GetValue()
        {
            return GetValue(0);
        }

        public abstract T GetValue(int pollerIndex);

        object IMonitor.GetValue()
        {
            return GetValue();
        }

        object IMonitor.GetValue(int pollerIndex)
        {
            return GetValue(pollerIndex);
        }
    }
}