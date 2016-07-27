using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elders.Servo.NET.Util
{
    /**
  * A {@link Clock} that provides a way to modify the time returned by
  * {@link System#currentTimeMillis()}.
  * <p/>
  * This can be used during application shutdown to force the clock forward and get the
  * latest values which normally
  * would not be returned until the next step boundary is crossed.
  */
    public class ClockWithOffset : Clock
    {
        ClockWithOffset() { }
        private static ClockWithOffset instance;
        public static ClockWithOffset INSTANCE
        {
            get
            {
                if (instance == null)
                {
                    instance = new ClockWithOffset();
                }
                return instance;
            }
        }
        private long offset = 0L;

        /**
         * Sets the offset for the clock.
         *
         * @param offset Number of milliseconds to add to the current time.
         */
        public void setOffset(long offset)
        {
            if (offset >= 0)
            {
                this.offset = offset;
            }
        }

        /**
         * {@inheritDoc}
         */
        public long now()
        {
            return offset + DateTime.UtcNow.Ticks / TimeSpan.TicksPerMillisecond;
        }

        public long WALL()
        {
            return DateTime.UtcNow.Ticks / TimeSpan.TicksPerMillisecond;
        }
    }
}
