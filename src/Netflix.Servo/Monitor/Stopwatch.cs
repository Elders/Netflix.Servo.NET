using System;

namespace Netflix.Servo.Monitor
{
    /**
 * Measures the time taken for execution of some code.
 */
    public interface Stopwatch
    {

        /**
         * Mark the start time.
         */
        void start();

        /**
         * Mark the end time.
         */
        void stop();

        /**
         * Reset the stopwatch so that it can be used again.
         */
        void reset();

        /**
         * Returns the duration in the specified time unit.
         */
        long getDuration(TimeSpan timeUnit);

        /**
         * Returns the duration in nanoseconds.
         */
        long getDuration();
    }
}
