using System.Collections.Generic;
using Netflix.Servo.Monitor;

namespace Netflix.Servo
{
    /**
 * Registry to keep track of objects with
 * {@link com.netflix.servo.annotations.Monitor} annotations.
 */
    public interface MonitorRegistry
    {
        /**
         * The set of registered Monitor objects.
         */
        ICollection<Monitor<T>> getRegisteredMonitors<T>();

        /**
         * Register a new monitor in the registry.
         */
        void register<T>(Monitor<T> monitor);

        /**
         * Unregister a Monitor from the registry.
         */
        void unregister<T>(Monitor<T> monitor);

        /**
         * Check whether a monitor has been registerd.
         */
        bool isRegistered<T>(Monitor<T> monitor);
    }
}
