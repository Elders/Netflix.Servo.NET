using System;
using Netflix.Servo.Attributes;
using Netflix.Servo.Util;

namespace Netflix.Servo.Monitor
{
    /**
 * A simple counter implementation backed by a StepLong. The value returned is a rate for the
 * previous interval as defined by the step.
 */
    public class StepCounter : AbstractMonitor<object>, ICounter<object>
    {

        private StepLong count;

        /**
         * Creates a new instance of the counter.
         */
        public StepCounter(MonitorConfig config)
           : this(config, ClockWithOffset.INSTANCE)
        {

        }

        /**
         * Creates a new instance of the counter.
         */
        //  @VisibleForTesting
        public StepCounter(MonitorConfig config, Clock clock)
            : base(config.withAdditionalTag(DataSourceType.NORMALIZED))
        {
            // This class will reset the value so it is not a monotonically increasing value as
            // expected for type=COUNTER. This class looks like a counter to the user and a gauge to
            // the publishing pipeline receiving the value.

            count = new StepLong(0L, clock);
        }


        public void increment()
        {
            count.addAndGet(1L);
        }

        public void increment(long amount)
        {
            if (amount > 0L)
            {
                count.addAndGet(amount);
            }
        }

        public override object GetValue(int pollerIndex)
        {
            Datapoint dp = count.poll(pollerIndex);
            double stepSeconds = Pollers.POLLING_INTERVALS[pollerIndex] / 1000.0;
            return dp.isUnknown() ? Double.NaN : dp.getValue() / stepSeconds;
        }

        /**
         * Get the count for the last completed polling interval for the given poller index.
         */
        public long getCount(int pollerIndex)
        {
            return count.poll(pollerIndex).getValue();
        }

        /**
         * Get the current count for the given poller index.
         */
        //@VisibleForTesting
        public long getCurrentCount(int pollerIndex)
        {
            return count.getCurrent(pollerIndex).Value;
        }

        public override String ToString()
        {
            return "StepCounter{config=" + config + ", count=" + count + '}';
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

            StepCounter that = (StepCounter)o;
            return config.Equals(that.config) && getCount(0) == that.getCount(0);
        }

        public override int GetHashCode()
        {
            return config.GetHashCode();
        }
    }
}
