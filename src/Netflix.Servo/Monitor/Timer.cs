using System;

namespace Netflix.Servo.Monitor
{
    /**
  * Monitor type for tracking how much time something is taking.
  */
    public interface Timer : NumericMonitor<long>
    {

        /**
         * Returns a stopwatch that has been started and will automatically
         * record its result to this timer when stopped.
         */
        Stopwatch start();

        /**
         * The time unit reported by this timer.
         */
        TimeSpan getTimeUnit();

        /**
         * Record a new value for this timer.
         *
         * @deprecated Use record(duration, timeUnit). By always providing a timeUnit to record()
         * you can have a base time unit of seconds, but
         * use recordings with timeunit of milliseconds for example.
         */
        [Obsolete]
        void record(long duration);

        /**
         * Record a new value that was collected with the given TimeUnit.
         */
        void record(TimeSpan duration);
    }
}
