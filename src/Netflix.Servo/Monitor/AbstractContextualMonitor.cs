using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Netflix.Servo.Tag;

namespace Netflix.Servo.Monitor
{
    /**
 * Base class used to simplify creation of contextual monitors.
 */
    public abstract class AbstractContextualMonitor<T, M> : CompositeMonitor<T>
        where M : Monitor<T>
    {

        /**
         * Base configuration shared across all contexts.
         */
        protected readonly MonitorConfig baseConfig;

        /**
         * Context to query when accessing a monitor.
         */
        protected readonly TaggingContext context;

        /**
         * Factory function used to create a new instance of a monitor.
         */
        protected Func<MonitorConfig, M> newMonitor;

        /**
         * Thread-safe map keeping track of the distinct monitors that have been created so far.
         */
        protected ConcurrentDictionary<MonitorConfig, M> monitors;

        /**
         * Create a new instance of the monitor.
         *
         * @param baseConfig shared configuration
         * @param context    provider for context specific tags
         * @param newMonitor function to create new monitors
         */
        protected AbstractContextualMonitor(MonitorConfig baseConfig, TaggingContext context, Func<MonitorConfig, M> newMonitor)
        {
            this.baseConfig = baseConfig;
            this.context = context;
            this.newMonitor = newMonitor;

            monitors = new ConcurrentDictionary<MonitorConfig, M>();
        }

        public T getValue()
        {
            return getValue(0);
        }

        /**
         * Returns a monitor instance for the current context. If no monitor exists for the current
         * context then a new one will be created.
         */
        protected M getMonitorForCurrentContext()
        {
            MonitorConfig contextConfig = getConfig();
            M monitor = monitors.GetOrAdd(contextConfig, ctx => newMonitor(ctx));
            return monitor;
        }

        public MonitorConfig getConfig()
        {
            TagList contextTags = context.getTags();
            return MonitorConfig.builder(baseConfig.getName())
                .withTags(baseConfig.getTags())
                .withTags(contextTags)
                .build();
        }

        public List<Monitor<T>> getMonitors()
        {
            return monitors.Values.Cast<Monitor<T>>().ToList();
        }

        public abstract T getValue(int pollerIndex);
    }
}
