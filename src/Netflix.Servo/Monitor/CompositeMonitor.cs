using System.Collections.Generic;

namespace Netflix.Servo.Monitor
{
    /// <summary>
    /// Used as a mixin for monitors that are composed of a number of sub-monitors.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface CompositeMonitor<T> : Monitor<T>
    {
        /// <summary>
        /// Returns a list of sub-monitors for this composite.
        /// </summary>
        /// <returns></returns>
        List<Monitor<T>> getMonitors();
    }
}
