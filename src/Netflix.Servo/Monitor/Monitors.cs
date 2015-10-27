////using System;
////using System.Collections.Generic;
////using System.Linq;
////using System.Text;
////using System.Threading.Tasks;

//using System;
//using System.Collections.Generic;
//using Netflix.Servo.Tag;

//namespace Netflix.Servo.Monitor
//{
//    /**
//  * Some helper functions for creating monitor objects.
//  */
//    public class Monitors
//    {
//        /// <summary>
//        /// Name used for composite objects that do not have an explicit id.
//        /// </summary>
//        private static string DEFAULT_ID = "default";

//        //        /**
//        //         * Function to create basic timers.
//        //         */
//        //        private class TimerFactory //: Function<MonitorConfig, Timer>
//        //        {
//        //            private TimeSpan unit;

//        //            public TimerFactory(TimeSpan unit)
//        //            {
//        //                this.unit = unit;
//        //            }

//        //            public Timer apply(MonitorConfig config)
//        //            {
//        //                return new BasicTimer(config, unit);
//        //            }
//        //        }

//        private static Func<MonitorConfig, Counter> COUNTER_FUNCTION = config => new BasicCounter(config);

//        private Monitors()
//        {
//        }

//        ///**
//        // * Create a new timer with only the name specified.
//        // */
//        //public static Timer newTimer(String name)
//        //{
//        //    return newTimer(name, TimeUnit.MILLISECONDS);
//        //}

//        //        /**
//        //         * Create a new timer with a name and context. The returned timer will maintain separate
//        //         * sub-monitors for each distinct set of tags returned from the context on an update operation.
//        //         */
//        //        public static Timer newTimer(String name, TaggingContext context)
//        //        {
//        //            return newTimer(name, TimeUnit.MILLISECONDS, context);
//        //        }

//        //        /**
//        //         * Create a new timer with only the name specified.
//        //         */
//        //        public static Timer newTimer(String name, TimeUnit unit)
//        //        {
//        //            return new BasicTimer(MonitorConfig.builder(name).build(), unit);
//        //        }

//        //        /**
//        //         * Create a new timer with a name and context. The returned timer will maintain separate
//        //         * sub-monitors for each distinct set of tags returned from the context on an update operation.
//        //         */
//        //        public static Timer newTimer(String name, TimeUnit unit, TaggingContext context)
//        //        {
//        //            final MonitorConfig config = MonitorConfig.builder(name).build();
//        //            return new ContextualTimer(config, context, new TimerFactory(unit));
//        //        }

//        /**
//         * Create a new counter instance.
//         */
//        public static Counter newCounter(String name)
//        {
//            return new BasicCounter(MonitorConfig.builder(name).build());
//        }

//        /**
//         * Create a new counter with a name and context. The returned counter will maintain separate
//         * sub-monitors for each distinct set of tags returned from the context on an update operation.
//         */
//        public static ICounter newCounter(String name, TaggingContext context)
//        {
//            MonitorConfig config = MonitorConfig.builder(name).build();
//            return new ContextualCounter(config, context, COUNTER_FUNCTION);
//        }

//        /**
//         * Helper function to easily create a composite for all monitor fields and
//         * annotated attributes of a given object.
//         */
//        public static CompositeMonitor<object> newObjectMonitor(Object obj)
//        {
//            return newObjectMonitor(null, obj);
//        }

//        /**
//         * Helper function to easily create a composite for all monitor fields and
//         * annotated attributes of a given object.
//         *
//         * @param id  a unique id associated with this particular instance of the
//         *            object. If multiple objects of the same class are registered
//         *            they will have the same config and conflict unless the id
//         *            values are distinct.
//         * @param obj object to search for monitors on. All fields of type
//         *            {@link Monitor} and fields/methods with a
//         *            {@link com.netflix.servo.annotations.Monitor} annotation
//         *            will be extracted and returned using
//         *            {@link CompositeMonitor#getMonitors()}.
//         * @return composite monitor based on the fields of the class
//         */
//        public static CompositeMonitor<object> newObjectMonitor(String id, Object obj)
//        {
//            ITagList tags = getMonitorTags(obj);

//            List<IMonitor> monitors = new List<IMonitor>();
//            addMonitors(monitors, id, tags, obj);

//            Type c = obj.GetType();
//            String objectId = (id == null) ? DEFAULT_ID : id;
//            return new BasicCompositeMonitor(newObjectConfig(c, objectId, tags), monitors);
//        }

//        //        /**
//        //         * Creates a new monitor for a thread pool with standard metrics for the pool size, queue size,
//        //         * task counts, etc.
//        //         *
//        //         * @param id   id to differentiate metrics for this pool from others.
//        //         * @param pool thread pool instance to monitor.
//        //         * @return composite monitor based on stats provided for the pool
//        //         */
//        //        public static CompositeMonitor<?> newThreadPoolMonitor(String id, ThreadPoolExecutor pool)
//        //        {
//        //            return newObjectMonitor(id, new MonitoredThreadPool(pool));
//        //        }

//        //        /**
//        //         * Creates a new monitor for a cache with standard metrics for the hits, misses, and loads.
//        //         *
//        //         * @param id    id to differentiate metrics for this cache from others.
//        //         * @param cache cache instance to monitor.
//        //         * @return composite monitor based on stats provided for the cache
//        //         */
//        //        public static CompositeMonitor<?> newCacheMonitor(String id, Cache<?, ?> cache)
//        //        {
//        //            return newObjectMonitor(id, new MonitoredCache(cache));
//        //        }

//        //        /**
//        //         * Register an object with the default registry. Equivalent to
//        //         * {@code DefaultMonitorRegistry.getInstance().register(Monitors.newObjectMonitor(obj))}.
//        //         */
//        //        public static void registerObject(Object obj)
//        //        {
//        //            registerObject(null, obj);
//        //        }

//        //        /**
//        //         * Unregister an object from the default registry. Equivalent to
//        //         * {@code DefaultMonitorRegistry.getInstance().unregister(Monitors.newObjectMonitor(obj))}.
//        //         *
//        //         * @param obj Previously registered using {@code Monitors.registerObject(obj)}
//        //         */
//        //        public static void unregisterObject(Object obj)
//        //        {
//        //            unregisterObject(null, obj);
//        //        }

//        //        /**
//        //         * Unregister an object from the default registry. Equivalent to
//        //         * {@code DefaultMonitorRegistry.getInstance().unregister(Monitors.newObjectMonitor(id, obj))}.
//        //         *
//        //         * @param obj Previously registered using {@code Monitors.registerObject(id, obj)}
//        //         */
//        //        public static void unregisterObject(String id, Object obj)
//        //        {
//        //            DefaultMonitorRegistry.getInstance().unregister(newObjectMonitor(id, obj));
//        //        }

//        //        /**
//        //         * Register an object with the default registry. Equivalent to
//        //         * {@code DefaultMonitorRegistry.getInstance().register(Monitors.newObjectMonitor(id, obj))}.
//        //         */
//        //        public static void registerObject(String id, Object obj)
//        //        {
//        //            DefaultMonitorRegistry.getInstance().register(newObjectMonitor(id, obj));
//        //        }

//        //        /**
//        //         * Check whether an object is currently registered with the default registry.
//        //         */
//        //        public static boolean isObjectRegistered(Object obj)
//        //        {
//        //            return isObjectRegistered(null, obj);
//        //        }

//        //        /**
//        //         * Check whether an object is currently registered with the default registry.
//        //         */
//        //        public static boolean isObjectRegistered(String id, Object obj)
//        //        {
//        //            return DefaultMonitorRegistry.getInstance().isRegistered(newObjectMonitor(id, obj));
//        //        }

//        //  /**
//        //   * Returns a new monitor that adds the provided tags to the configuration returned by the
//        //   * wrapped monitor.
//        //   */
//        //  @SuppressWarnings("unchecked")
//        //      static <T> Monitor<T> wrap(TagList tags, Monitor<T> monitor)
//        //        {
//        //            Monitor<T> m;
//        //            if (monitor instanceof CompositeMonitor<?>) {
//        //                m = new CompositeMonitorWrapper<>(tags, (CompositeMonitor<T>)monitor);
//        //            } else if (monitor instanceof NumericMonitor<?>) {
//        //                m = (Monitor<T>)new NumericMonitorWrapper(tags, (NumericMonitor <?>) monitor);
//        //            } else {
//        //                m = new MonitorWrapper<>(tags, monitor);
//        //            }
//        //            return m;
//        //        }

//        /**
//         * Extract all monitors across class hierarchy.
//         */
//        static void addMonitors(List<IMonitor> monitors, String id, ITagList tags, Object obj)
//        {
//            //for (Class <?> c = obj.getClass(); c != null; c = c.getSuperclass())
//            //{
//            //    addMonitorFields(monitors, id, tags, obj, c);
//            //    addAnnotatedFields(monitors, id, tags, obj, c);
//            //}
//        }

//        //        /**
//        //         * Extract all fields of {@code obj} that are of type {@link Monitor} and add them to
//        //         * {@code monitors}.
//        //         */
//        //        static void addMonitorFields(
//        //            List<Monitor<?>> monitors, String id, TagList tags, Object obj, Class<?> c)
//        //        {
//        //            try
//        //            {
//        //                final SortedTagList.Builder builder = SortedTagList.builder();
//        //                builder.withTag("class", className(obj.getClass()));
//        //                if (tags != null)
//        //                {
//        //                    builder.withTags(tags);
//        //                }
//        //                if (id != null)
//        //                {
//        //                    builder.withTag("id", id);
//        //                }
//        //                final TagList classTags = builder.build();

//        //                final Field[] fields = c.getDeclaredFields();
//        //                for (Field field : fields)
//        //                {
//        //                    if (isMonitorType(field.getType()))
//        //                    {
//        //                        field.setAccessible(true);
//        //                        final Monitor<?> m = (Monitor <?>) field.get(obj);
//        //                        if (m == null)
//        //                        {
//        //                            throw new NullPointerException("field " + field.getName()
//        //                                + " in class " + c.getName() + " is null, all monitor fields must be"
//        //                                + " initialized before registering");
//        //                        }
//        //                        monitors.add(wrap(classTags, m));
//        //                    }
//        //                }
//        //            }
//        //            catch (Exception e)
//        //            {
//        //                throw Throwables.propagate(e);
//        //            }
//        //        }

//        //        /**
//        //         * Extract all fields/methods of {@code obj} that have a monitor annotation and add them to
//        //         * {@code monitors}.
//        //         */
//        //        static void addAnnotatedFields(
//        //            List<Monitor<?>> monitors, String id, TagList tags, Object obj, Class<?> c)
//        //        {
//        //            final Class< com.netflix.servo.annotations.Monitor > annoClass =
//        //                com.netflix.servo.annotations.Monitor.class;
//        //    try {
//        //      Field[] fields = c.getDeclaredFields();
//        //      for (Field field : fields) {
//        //        final com.netflix.servo.annotations.Monitor anno = field.getAnnotation(annoClass);
//        //        if (anno != null) {
//        //          final MonitorConfig config =
//        //              newConfig(obj.getClass(), field.getName(), id, anno, tags);
//        //          if (anno.type() == DataSourceType.INFORMATIONAL) {
//        //            monitors.add(new AnnotatedStringMonitor(config, obj, field));
//        //          } else {
//        //            checkType(anno, field.getType(), c);
//        //            monitors.add(new AnnotatedNumberMonitor(config, obj, field));
//        //          }
//        //        }
//        //      }

//        //      Method[] methods = c.getDeclaredMethods();
//        //      for (Method method : methods) {
//        //        final com.netflix.servo.annotations.Monitor anno = method.getAnnotation(annoClass);
//        //        if (anno != null) {
//        //          final MonitorConfig config =
//        //              newConfig(obj.getClass(), method.getName(), id, anno, tags);
//        //          if (anno.type() == DataSourceType.INFORMATIONAL) {
//        //            monitors.add(new AnnotatedStringMonitor(config, obj, method));
//        //          } else {
//        //            checkType(anno, method.getReturnType(), c);
//        //            monitors.add(new AnnotatedNumberMonitor(config, obj, method));
//        //          }
//        //        }
//        //      }
//        //    } catch (Exception e) {
//        //      throw Throwables.propagate(e);
//        //    }
//        //  }

//        /**
//         * Get tags from annotation.
//         */
//        private static ITagList getMonitorTags(Object obj)
//        {
//            //            try
//            //            {
//            //                Type c = obj.GetType();
//            //                Field[] fields = c.getDeclaredFields();
//            //                for (Field field : fields)
//            //                {
//            //                    final MonitorTags anno = field.getAnnotation(MonitorTags.class);
//            //                if (anno != null) {
//            //                  field.setAccessible(true);
//            //                  return (TagList) field.get(obj);
//            //                }
//            //}

//            //Method[] methods = c.getDeclaredMethods();
//            //              for (Method method : methods) {
//            //                final MonitorTags anno = method.getAnnotation(MonitorTags.class);
//            //                if (anno != null) {
//            //                  method.setAccessible(true);
//            //                  return (TagList) method.invoke(obj);
//            //                }
//            //              }
//            //            } catch (Exception e) {
//            //              throw Throwables.propagate(e);
//            //            }

//            return null;
//        }

//        //  /**
//        //   * Verify that the type for the annotated field is numeric.
//        //   */
//        //  private static void checkType(
//        //      com.netflix.servo.annotations.Monitor anno, Class<?> type, Class<?> container)
//        //{
//        //    if (!isNumericType(type))
//        //    {
//        //        final String msg = "annotation of type " + anno.type().name() + " can only be used"
//        //            + " with numeric values, " + anno.name() + " in class " + container.getName()
//        //            + " is applied to a field or method of type " + type.getName();
//        //        throw new IllegalArgumentException(msg);
//        //    }
//        //}

//        ///**
//        // * Returns true if {@code c} can be assigned to a number.
//        // */
//        //private static boolean isNumericType(Class<?> c)
//        //{
//        //    return Number.class.isAssignableFrom(c)
//        //        || double.class == c
//        //        || float.class == c
//        //        || long.class == c
//        //        || int.class == c
//        //        || short.class == c
//        //        || byte.class == c;
//        //  }

//        //  /**
//        //   * Returns true if {@code c} can be assigned to a monitor.
//        //   */
//        //  private static boolean isMonitorType(Class<?> c)
//        //{
//        //    return Monitor.class.isAssignableFrom(c);
//        //  }

//        /**
//         * Creates a monitor config for a composite object.
//         */
//        private static MonitorConfig newObjectConfig(Type c, String id, ITagList tags)
//        {
//            MonitorConfig.Builder builder = MonitorConfig.builder(id);
//            builder.withTag("class", c.Name);
//            if (tags != null)
//            {
//                builder.withTags(tags);
//            }
//            return builder.build();
//        }

//        //private static String className(Class c)
//        //{
//        //    final String simpleName = c.getSimpleName();

//        //    return simpleName.isEmpty() ? className(c.getEnclosingClass()) : simpleName;
//        //}

//        ///**
//        // * Creates a monitor config based on an annotation.
//        // */
//        //private static MonitorConfig newConfig(
//        //    Class<?> c,
//        //    String defaultName,
//        //    String id,
//        //    com.netflix.servo.annotations.Monitor anno,
//        //    TagList tags)
//        //{
//        //    String name = anno.name();
//        //    if (name.isEmpty())
//        //    {
//        //        name = defaultName;
//        //    }
//        //    MonitorConfig.Builder builder = MonitorConfig.builder(name);
//        //    builder.withTag("class", className(c));
//        //    builder.withTag(anno.type());
//        //    builder.withTag(anno.level());
//        //    if (tags != null)
//        //    {
//        //        builder.withTags(tags);
//        //    }
//        //    if (id != null)
//        //    {
//        //        builder.withTag("id", id);
//        //    }
//        //    return builder.build();
//        //}
//    }
//}
