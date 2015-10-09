using System;
using Java.Util.Concurrent;
using Netflix.Servo.Attributes;

namespace Netflix.Servo.Monitor
{
    /**
 * A gauge implementation that invokes a specified callable to get the current value.
 */
    public class BasicGauge<T> : AbstractMonitor<T>, Gauge<T>
    {
        private Callable<T> function;

        /**
         * Creates a new instance of the gauge.
         *
         * @param config   configuration for this monitor
         * @param function a function used to fetch the value on demand
         */
        public BasicGauge(MonitorConfig config, Callable<T> function)
            : base(config.withAdditionalTag(DataSourceType.GAUGE))
        {
            this.function = function;
        }

        public override T getValue(int pollerIndex)
        {
            return function.Call();
        }

        public override bool Equals(Object obj)
        {
            if (obj == null || !(obj is BasicGauge<T>))
            {
                return false;
            }
            BasicGauge<T> m = (BasicGauge<T>)obj;
            return config.Equals(m.getConfig()) && function.Equals(m.function);
        }

        public override int GetHashCode()
        {
            int result = config.GetHashCode();
            result = 31 * result + function.GetHashCode();
            return result;
        }

        public override String ToString()
        {
            return "BasicGauge{config=" + config + ", function=" + function + '}';
        }
    }
}
