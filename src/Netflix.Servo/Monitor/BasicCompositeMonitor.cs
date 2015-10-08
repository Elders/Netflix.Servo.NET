using System;
using System.Collections.Generic;
using System.Linq;

namespace Netflix.Servo.Monitor
{
    /**
 * Simple composite monitor type with a static list of sub-monitors. The value for the composite
 * is the number of sub-monitors.
 */
    public class BasicCompositeMonitor : AbstractMonitor<int>, CompositeMonitor<int>
    {
        private readonly List<Monitor<int>> monitors;

        /**
         * Create a new composite.
         *
         * @param config   configuration for the composite. It is recommended that the configuration
         *                 shares common tags with the sub-monitors, but it is not enforced.
         * @param monitors list of sub-monitors
         */
        public BasicCompositeMonitor(MonitorConfig config, List<Monitor<int>> monitors)
            : base(config)
        {
            this.monitors = new List<Monitor<int>>(monitors);
        }

        public override int getValue(int pollerIndex)
        {
            return monitors.Count();
        }


        public List<Monitor<int>> getMonitors()
        {
            return monitors;
        }


        public override bool Equals(Object obj)
        {
            if (obj == null || !(obj is BasicCompositeMonitor))
            {
                return false;
            }
            BasicCompositeMonitor m = (BasicCompositeMonitor)obj;
            return config.Equals(m.getConfig()) && monitors.Equals(m.getMonitors());
        }

        public override int GetHashCode()
        {
            int result = config.GetHashCode();
            result = 31 * result + monitors.GetHashCode();
            return result;
        }

        public override String ToString()
        {
            return "BasicCompositeMonitor{config=" + config + ", monitors=" + monitors + '}';
        }
    }
}
