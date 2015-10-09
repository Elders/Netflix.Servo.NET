namespace Netflix.Servo.Monitor
{
    /**
 * Monitor type for tracking how often some event is occurring.
 */
    public interface Counter : NumericMonitor<object>
    {
        /**
         * Update the count by one.
         */
        void increment();

        /**
         * Update the count by the specified amount.
         */
        void increment(long amount);
    }
}
