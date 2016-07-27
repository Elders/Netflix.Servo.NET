namespace Elders.Servo.NET.Monitor
{
    /// <summary>
    /// A monitor type that has a numeric value.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface INumericMonitor<out T> : IMonitor<T>
    {
    }
}