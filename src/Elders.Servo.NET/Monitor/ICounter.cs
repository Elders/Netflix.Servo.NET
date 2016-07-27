namespace Elders.Servo.NET.Monitor
{

    /// <summary>
    /// Monitor type for tracking how often some event is occurring.
    /// </summary>
    public interface ICounter<T> : INumericMonitor<T>
    {
        /// <summary>
        /// Update the count by one.
        /// </summary>
        void increment();

        /// <summary>
        /// Update the count by the specified amount.
        /// </summary>
        /// <param name="amount">The amount.</param>
        void increment(long amount);
    }
}