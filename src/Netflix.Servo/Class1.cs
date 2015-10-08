//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Netflix.Servo
//{
//    /**
// * Registry to keep track of objects with
// * {@link com.netflix.servo.annotations.Monitor} annotations.
// */
//    public interface MonitorRegistry
//    {
//        /**
//         * The set of registered Monitor objects.
//         */
//        Collection<Monitor<?>> getRegisteredMonitors();

//        /**
//         * Register a new monitor in the registry.
//         */
//        void register(Monitor<?> monitor);

//        /**
//         * Unregister a Monitor from the registry.
//         */
//        void unregister(Monitor<?> monitor);

//        /**
//         * Check whether a monitor has been registerd.
//         */
//        boolean isRegistered(Monitor<?> monitor);
//    }
//}
