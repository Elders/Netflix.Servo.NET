using Netflix.Servo.Util;

namespace Netflix.Servo.Monitor
{
    /**
 * Stopwatch that will also record to a timer.
 */
    public class TimedStopwatch : BasicStopwatch
    {
        private Timer timer;

        /**
         * Create a new instance with the specified timer.
         */
        public TimedStopwatch(Timer timer)
        {
            Preconditions.checkNotNull(timer, "timer");
            this.timer = timer;
        }

        public override void stop()
        {
            base.stop();
            timer.record(getDuration());
        }
    }
}
