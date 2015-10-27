using System.Collections.Concurrent;
using System.Collections.Generic;
using Netflix.Servo.Monitor;

namespace Netflix.Servo
{
    /**
  * Registry to keep track of objects with
  * {@link com.netflix.servo.annotations.Monitor} annotations.
  */
    public interface IMonitorRegistry
    {
        /**
         * The set of registered Monitor objects.
         */
        ICollection<IMonitor> getRegisteredMonitors();

        /**
         * Register a new monitor in the registry.
         */
        void register(IMonitor monitor);

        /**
         * Unregister a Monitor from the registry.
         */
        void unregister(IMonitor monitor);

        /**
         * Check whether a monitor has been registerd.
         */
        bool isRegistered(IMonitor monitor);
    }

    /**
 * Default registry that delegates all actions to a class specified by the
 * {@code com.netflix.servo.DefaultMonitorRegistry.registryClass} property. The
 * specified registry class must have a constructor with no arguments. If the
 * property is not specified or the class cannot be loaded an instance of
 * {@link com.netflix.servo.jmx.JmxMonitorRegistry} will be used.
 * <p/>
 * If the default {@link com.netflix.servo.jmx.JmxMonitorRegistry} is used, the property
 * {@code com.netflix.servo.DefaultMonitorRegistry.jmxMapperClass} can optionally be
 * specified to control how monitors are mapped to JMX {@link javax.management.ObjectName}.
 * This property specifies the {@link com.netflix.servo.jmx.ObjectNameMapper}
 * implementation class to use. The implementation must have a constructor with
 * no arguments.
 */
    public class DefaultMonitorRegistry : IMonitorRegistry
    {

        //private static ILogger LOG = LoggerFactory.GetLogger(typeof(DefaultMonitorRegistry));
        //private static String CLASS_NAME = typeof(DefaultMonitorRegistry).FullName;
        //private static String REGISTRY_CLASS_PROP = CLASS_NAME + ".registryClass";
        //private static string REGISTRY_NAME_PROP = CLASS_NAME + ".registryName";
        //private static String REGISTRY_JMX_NAME_PROP = CLASS_NAME + ".jmxMapperClass";
        private static IMonitorRegistry INSTANCE = new DefaultMonitorRegistry();
        //private static String DEFAULT_REGISTRY_NAME = "com.netflix.servo";

        private IMonitorRegistry registry;

        /**
         * Returns the instance of this registry.
         */
        public static IMonitorRegistry getInstance()
        {
            return INSTANCE;
        }

        /**
         * Creates a new instance based on system properties.
         */
        DefaultMonitorRegistry() //: this(System.getProperties())
        {
            registry = new mynkow();
        }

        /**
         * Creates a new instance based on the provide properties object. Only
         * intended for use in unit tests.
         */
        //      DefaultMonitorRegistry(Properties props)
        //      {
        //          String className = props.getProperty(REGISTRY_CLASS_PROP);
        //          String registryName = props.getProperty(REGISTRY_NAME_PROP, DEFAULT_REGISTRY_NAME);
        //          if (className != null)
        //          {
        //              MonitorRegistry r;
        //              try
        //              {
        //                  Class <?> c = Class.forName(className);
        //                  r = (MonitorRegistry)c.newInstance();
        //              }
        //              catch (Throwable t)
        //              {
        //                  LOG.error(
        //                      "failed to create instance of class " + className + ", "
        //                          + "using default class "
        //                          + JmxMonitorRegistry.class.getName(),
        //          t);
        //      r = new JmxMonitorRegistry(registryName);
        //  }
        //  registry = r;
        //  } else {
        //    registry = new JmxMonitorRegistry(registryName,
        //        getObjectNameMapper(props));
        //  }
        //}

        /**
         * The set of registered Monitor objects.
         */

        public ICollection<IMonitor> getRegisteredMonitors()
        {
            return registry.getRegisteredMonitors();
        }

        /**
         * Register a new monitor in the registry.
         */
        public void register(IMonitor monitor)
        {
            registry.register(monitor);
        }

        /**
         * Unregister a Monitor from the registry.
         */
        public void unregister(IMonitor monitor)
        {
            registry.unregister(monitor);
        }

        /**
         * Returns the inner registry that was created to service the requests.
         */
        public IMonitorRegistry getInnerRegistry()
        {
            return registry;
        }


        public bool isRegistered(IMonitor monitor)
        {
            return registry.isRegistered(monitor);
        }


        public class mynkow : IMonitorRegistry
        {
            readonly ConcurrentDictionary<MonitorConfig, IMonitor> monitors;

            public mynkow()
            {
                monitors = new ConcurrentDictionary<MonitorConfig, IMonitor>();
            }

            public ICollection<IMonitor> getRegisteredMonitors()
            {
                return monitors.Values;
            }

            public bool isRegistered(IMonitor monitor)
            {
                return monitors.ContainsKey(monitor.getConfig());
            }

            public void register(IMonitor monitor)
            {
                monitors.TryAdd(monitor.getConfig(), monitor);
            }

            public void unregister(IMonitor monitor)
            {
                IMonitor removed;
                monitors.TryRemove(monitor.getConfig(), out removed);
            }
        }
    }
}
