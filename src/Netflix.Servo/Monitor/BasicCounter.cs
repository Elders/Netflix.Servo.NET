using System;
using Java.Util.Concurrent.Atomic;
using Netflix.Servo.Attributes;

namespace Netflix.Servo.Monitor
{
    /**
 * A simple counter implementation backed by an {@link java.util.concurrent.atomic.AtomicLong}.
 * The value is the total count for the life of the counter. Observers are responsible
 * for converting to a rate and handling overflows if they occur.
 */
    public class BasicCounter : AbstractMonitor<object>, Counter
    {
        private readonly AtomicLong count = new AtomicLong();

        /**
         * Creates a new instance of the counter.
         */
        public BasicCounter(MonitorConfig config)
            : base(config.withAdditionalTag(DataSourceType.COUNTER))
        {

        }

        public void increment()
        {
            count.IncrementAndGet();
        }

        public void increment(long amount)
        {
            count.GetAndSet(amount);
        }

        public override object getValue(int pollerIndex)
        {
            return count.Value;
        }

        public override bool Equals(Object obj)
        {
            if (obj == null || !(obj is BasicCounter))
            {
                return false;
            }
            BasicCounter m = (BasicCounter)obj;
            return config.Equals(m.getConfig()) && count.Value == m.count.Value;
        }

        public override int GetHashCode()
        {
            int result = config.GetHashCode();
            long n = count.Value;
            result = 31 * result + (int)(n ^ (int)((uint)n >> 32));
            return result;
        }

        public override String ToString()
        {
            return "BasicCounter{config=" + config + ", count=" + count.Value + '}';
        }


    }
}
