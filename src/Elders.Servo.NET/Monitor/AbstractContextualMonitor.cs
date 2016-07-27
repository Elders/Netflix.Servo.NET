//using System;
//using System.Collections.Concurrent;
//using System.Collections.Generic;
//using System.Linq;
//using Elders.Servo.NET.Tag;

//namespace Elders.Servo.NET.Monitor
//{
//    /**
// * Base class used to simplify creation of contextual monitors.
// */
//    public abstract class AbstractContextualMonitor<T> : ICompositeMonitor
//    {

//        /**
//         * Base configuration shared across all contexts.
//         */
//        protected readonly MonitorConfig baseConfig;

//        /**
//         * Context to query when accessing a monitor.
//         */
//        protected readonly TaggingContext context;

//        /**
//         * Factory function used to create a new instance of a monitor.
//         */
//        protected Func<MonitorConfig, IMonitor> newMonitor;

//        /**
//         * Thread-safe map keeping track of the distinct monitors that have been created so far.
//         */
//        protected ConcurrentDictionary<MonitorConfig, IMonitor> monitors;

//        /**
//         * Create a new instance of the monitor.
//         *
//         * @param baseConfig shared configuration
//         * @param context    provider for context specific tags
//         * @param newMonitor function to create new monitors
//         */
//        protected AbstractContextualMonitor(MonitorConfig baseConfig, TaggingContext context, Func<MonitorConfig, IMonitor> newMonitor)
//        {
//            this.baseConfig = baseConfig;
//            this.context = context;
//            this.newMonitor = newMonitor;

//            monitors = new ConcurrentDictionary<MonitorConfig, IMonitor>();
//        }

//        public IMonitor GetValue()
//        {
//            return GetValue(0);
//        }

//        /**
//         * Returns a monitor instance for the current context. If no monitor exists for the current
//         * context then a new one will be created.
//         */
//        protected IMonitor getMonitorForCurrentContext()
//        {
//            MonitorConfig contextConfig = getConfig();
//            IMonitor monitor = monitors.GetOrAdd(contextConfig, ctx => newMonitor(ctx));
//            return monitor;
//        }

//        public MonitorConfig getConfig()
//        {
//            ITagList contextTags = context.getTags();
//            return MonitorConfig.builder(baseConfig.getName())
//                .withTags(baseConfig.getTags())
//                .withTags(contextTags)
//                .build();
//        }

//        public List<IMonitor> getMonitors()
//        {
//            return monitors.Values.ToList();
//        }

//        public abstract IMonitor GetValue(int pollerIndex);
//    }
//}
