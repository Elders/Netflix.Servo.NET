using System;
using Java.Util.Concurrent.Atomic;
using Netflix.Servo.Attributes;

namespace Netflix.Servo.Monitor
{
    /**
  * A simple informational implementation that maintains a string value.
  */
    public class BasicInformational : AbstractMonitor<String>, IInformational
    {
        private readonly AtomicReference<String> info = new AtomicReference<string>();

        /**
         * Creates a new instance of the counter.
         */
        public BasicInformational(MonitorConfig config)
            : base(config.withAdditionalTag(DataSourceType.INFORMATIONAL))
        {

        }

        /**
         * Set the value to show for this monitor.
         */
        public void setValue(String value)
        {
            info.GetAndSet(value);
        }

        public override String GetValue(int pollerIndex)
        {
            return info.Value;
        }

        public override bool Equals(Object o)
        {
            if (this == o)
            {
                return true;
            }
            if (o == null || !(o is BasicInformational))
            {
                return false;
            }
            BasicInformational that = (BasicInformational)o;

            String thisInfo = info.Value;
            String thatInfo = that.info.Value;
            return config.Equals(that.config)
                && (thisInfo == null ? thatInfo == null : thisInfo.Equals(thatInfo));
        }

        public override int GetHashCode()
        {
            int result = config.GetHashCode();
            int infoHashcode = info.Value != null ? info.Value.GetHashCode() : 0;
            result = 31 * result + infoHashcode;
            return result;
        }

        public override String ToString()
        {
            return "BasicInformational{config=" + config + ", info=" + info + '}';
        }
    }
}
