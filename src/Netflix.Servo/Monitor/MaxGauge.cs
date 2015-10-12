using System;
using Java.Util.Concurrent.Atomic;
using Netflix.Servo.Attributes;
using Netflix.Servo.Util;

namespace Netflix.Servo.Monitor
{
    /**
 * Gauge that keeps track of the maximum value seen since the last reset. Updates should be
 * non-negative, the reset value is 0.
 */
    public class MaxGauge : AbstractMonitor<long>, Gauge<long>
    {
        private StepLong max;

        /**
         * Creates a new instance of the gauge.
         */
        public MaxGauge(MonitorConfig config)
            : this(config, ClockWithOffset.INSTANCE)
        {

        }

        /**
         * Creates a new instance of the gauge using a specific clock. Useful for unit testing.
         */
        internal MaxGauge(MonitorConfig config, Clock clock)
            : base(config.withAdditionalTag(DataSourceType.GAUGE))
        {
            max = new StepLong(0L, clock);
        }

        /**
         * Update the max for the given index if the provided value is larger than the current max.
         */
        private void updateMax(int idx, long v)
        {
            AtomicLong current = max.getCurrent(idx);
            long m = current.Value;
            while (v > m)
            {
                if (current.CompareAndSet(m, v))
                {
                    break;
                }
                m = current.Value;
            }
        }

        /**
         * Update the max if the provided value is larger than the current max.
         */
        public void update(long v)
        {
            for (int i = 0; i < Pollers.NUM_POLLERS; ++i)
            {
                updateMax(i, v);
            }
        }

        public override long getValue(int nth)
        {
            return max.poll(nth).getValue();
        }

        /**
         * Returns the current max value since the last reset.
         */
        public long getCurrentValue(int nth)
        {
            return max.getCurrent(nth).Value;
        }


        public override bool Equals(Object obj)
        {
            if (this == obj)
            {
                return true;
            }
            if (obj == null || !(obj is MaxGauge))
            {
                return false;
            }
            MaxGauge m = (MaxGauge)obj;
            return config.Equals(m.getConfig()) && getValue(0).Equals(m.getValue(0));
        }

        public override int GetHashCode()
        {
            int result = getConfig().GetHashCode();
            result = 31 * result + getValue(0).GetHashCode();
            return result;
        }

        public override String ToString()
        {
            return "MaxGauge{config=" + config + ", max=" + max + '}';
        }
    }
}
