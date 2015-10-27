namespace Netflix.Servo.Monitor
{
    /// <summary>
    /// Monitor type that provides the current value, e.g., the percentage of disk space used.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IGauge<out T> : INumericMonitor<T>
    {
    }
}