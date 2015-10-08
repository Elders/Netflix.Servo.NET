//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Netflix.Servo.Monitor;
//using slf4net;

//namespace Netflix.Servo
//{
//    /**
// * Default registry that delegates all actions to a class specified by the
// * {@code com.netflix.servo.DefaultMonitorRegistry.registryClass} property. The
// * specified registry class must have a constructor with no arguments. If the
// * property is not specified or the class cannot be loaded an instance of
// * {@link com.netflix.servo.jmx.JmxMonitorRegistry} will be used.
// * <p/>
// * If the default {@link com.netflix.servo.jmx.JmxMonitorRegistry} is used, the property
// * {@code com.netflix.servo.DefaultMonitorRegistry.jmxMapperClass} can optionally be
// * specified to control how monitors are mapped to JMX {@link javax.management.ObjectName}.
// * This property specifies the {@link com.netflix.servo.jmx.ObjectNameMapper}
// * implementation class to use. The implementation must have a constructor with
// * no arguments.
// */
//    public class DefaultMonitorRegistry : MonitorRegistry
//    {

//        private static ILogger LOG = LoggerFactory.GetLogger(typeof(DefaultMonitorRegistry));
//        private static String CLASS_NAME = typeof(DefaultMonitorRegistry).FullName;
//        private static String REGISTRY_CLASS_PROP = CLASS_NAME + ".registryClass";
//        private static string REGISTRY_NAME_PROP = CLASS_NAME + ".registryName";
//        private static String REGISTRY_JMX_NAME_PROP = CLASS_NAME + ".jmxMapperClass";
//        private static MonitorRegistry INSTANCE = new DefaultMonitorRegistry();
//        private static String DEFAULT_REGISTRY_NAME = "com.netflix.servo";

//        private MonitorRegistry registry;

//        /**
//         * Returns the instance of this registry.
//         */
//        public static MonitorRegistry getInstance()
//        {
//            return INSTANCE;
//        }

//        /**
//         * Creates a new instance based on system properties.
//         */
//        DefaultMonitorRegistry() //: this(System.getProperties())
//        {
//        }

//        /**
//         * Creates a new instance based on the provide properties object. Only
//         * intended for use in unit tests.
//         */
//        //      DefaultMonitorRegistry(Properties props)
//        //      {
//        //          String className = props.getProperty(REGISTRY_CLASS_PROP);
//        //          String registryName = props.getProperty(REGISTRY_NAME_PROP, DEFAULT_REGISTRY_NAME);
//        //          if (className != null)
//        //          {
//        //              MonitorRegistry r;
//        //              try
//        //              {
//        //                  Class <?> c = Class.forName(className);
//        //                  r = (MonitorRegistry)c.newInstance();
//        //              }
//        //              catch (Throwable t)
//        //              {
//        //                  LOG.error(
//        //                      "failed to create instance of class " + className + ", "
//        //                          + "using default class "
//        //                          + JmxMonitorRegistry.class.getName(),
//        //          t);
//        //      r = new JmxMonitorRegistry(registryName);
//        //  }
//        //  registry = r;
//        //  } else {
//        //    registry = new JmxMonitorRegistry(registryName,
//        //        getObjectNameMapper(props));
//        //  }
//        //}

//        /**
//         * Gets the {@link ObjectNameMapper} to use by looking at the
//         * {@code com.netflix.servo.DefaultMonitorRegistry.jmxMapperClass}
//         * property. If not specified, then {@link ObjectNameMapper#DEFAULT}
//         * is used.
//         *
//         * @param props the properties
//         * @return the mapper to use
//         */
//        private static ObjectNameMapper getObjectNameMapper(Properties props)
//        {
//            ObjectNameMapper mapper = ObjectNameMapper.DEFAULT;
//            final String jmxNameMapperClass = props.getProperty(REGISTRY_JMX_NAME_PROP);
//            if (jmxNameMapperClass != null)
//            {
//                try
//                {
//                    Class <?> mapperClazz = Class.forName(jmxNameMapperClass);
//                    mapper = (ObjectNameMapper)mapperClazz.newInstance();
//                }
//                catch (Throwable t)
//                {
//                    LOG.error(
//                        "failed to create the JMX ObjectNameMapper instance of class "
//                            + jmxNameMapperClass
//                            + ", using the default naming scheme",
//                        t);
//                }
//            }

//            return mapper;
//        }

//        /**
//         * The set of registered Monitor objects.
//         */

//        public ICollection<Monitor<T>> getRegisteredMonitors<T>()
//        {
//            return registry.getRegisteredMonitors();
//        }

//        /**
//         * Register a new monitor in the registry.
//         */
//        public void register<T>(Monitor<T> monitor)
//        {
//            registry.register(monitor);
//        }

//        /**
//         * Unregister a Monitor from the registry.
//         */
//        public void unregister<T>(Monitor<T> monitor)
//        {
//            registry.unregister(monitor);
//        }

//        /**
//         * Returns the inner registry that was created to service the requests.
//         */
//        public MonitorRegistry getInnerRegistry()
//        {
//            return registry;
//        }


//        public bool isRegistered<T>(Monitor<T> monitor)
//        {
//            return registry.isRegistered(monitor);
//        }

//    }
