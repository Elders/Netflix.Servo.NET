using System;
using Java.Util.Concurrent.Atomic;
using Netflix.Servo.Attributes;
using Netflix.Servo.Util;

namespace Netflix.Servo.Monitor
{
    /**
 * A simple counter implementation backed by a StepLong but using doubles.
 * The value returned is a rate for the
 * previous interval as defined by the step.
 */
    internal class DoubleCounter : AbstractMonitor<double>, NumericMonitor<double>
    {

        private StepLong count;

        /**
         * Creates a new instance of the counter.
         */
        internal DoubleCounter(MonitorConfig config, Clock clock)
            : base(config.withAdditionalTag(DataSourceType.NORMALIZED))
        {
            // This class will reset the value so it is not a monotonically increasing value as
            // expected for type=COUNTER. This class looks like a counter to the user and a gauge to
            // the publishing pipeline receiving the value.

            count = new StepLong(0L, clock);
        }

        private void add(AtomicLong num, double amount)
        {
            long v;
            double d;
            long next;
            do
            {
                v = num.Value;
                d = (double)v;// Double.longBitsToDouble(v);
                next = (long)(d + amount);
            } while (!num.CompareAndSet(v, next));
        }

        /**
         * Increment the value by the specified amount.
         */
        internal void increment(double amount)
        {
            if (amount >= 0.0)
            {
                for (int i = 0; i < Pollers.NUM_POLLERS; ++i)
                {
                    add(count.getCurrent(i), amount);
                }
            }
        }

        public override double GetValue(int pollerIndex)
        {
            Datapoint dp = count.poll(pollerIndex);
            double stepSeconds = Pollers.POLLING_INTERVALS[pollerIndex] / 1000.0;
            return dp.isUnknown() ? Double.NaN : dp.getValue() / stepSeconds;
        }

        /**
         * Get the current count for the given poller index.
         */
        //@VisibleForTesting
        public double getCurrentCount(int pollerIndex)
        {
            return count.getCurrent(pollerIndex).Value;
        }

        public override String ToString()
        {
            return "DoubleCounter{"
                + "config=" + config
                + "count=" + count.getCurrent(0)
                + '}';
        }

        public override bool Equals(Object o)
        {
            if (this == o)
            {
                return true;
            }
            if (o == null || GetType() != o.GetType())
            {
                return false;
            }
            DoubleCounter that = (DoubleCounter)o;
            return config.Equals(that.config) && GetValue(0).Equals(that.GetValue(0));
        }

        public override int GetHashCode()
        {
            return 11 ^ config.GetHashCode() ^ GetValue(0).GetHashCode();
        }
    }
}
