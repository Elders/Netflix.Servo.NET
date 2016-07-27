using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Elders.Servo.NET.Monitor;
using Elders.Servo.NET.Tag;
using Elders.Servo.NET.Util;

namespace Elders.Servo.NET
{
    /**
     * Represents a metric value at a given point in time.
     */
    public sealed class Metric
    {
        private MonitorConfig config;
        private long timestamp;
        private Object value;

        /**
         * Creates a new instance.
         *
         * @param name      name of the metric
         * @param tags      tags associated with the metric
         * @param timestamp point in time when the metric value was sampled
         * @param value     value of the metric
         */
        public Metric(string name, ITagList tags, long timestamp, Object value)
            : this(new MonitorConfig.Builder(name).withTags(tags).build(), timestamp, value)
        {

        }

        /**
         * Creates a new instance.
         *
         * @param config    config settings associated with the metric
         * @param timestamp point in time when the metric value was sampled
         * @param value     value of the metric
         */
        public Metric(MonitorConfig config, long timestamp, Object value)
        {
            this.config = Preconditions.checkNotNull(config, "config");
            this.timestamp = timestamp;
            this.value = Preconditions.checkNotNull(value, "value");
        }

        /**
         * Returns the config settings associated with the metric.
         */
        public MonitorConfig getConfig()
        {
            return config;
        }

        /**
         * Returns the point in time when the metric was sampled.
         */
        public long getTimestamp()
        {
            return timestamp;
        }

        /**
         * Returns the value of the metric.
         */
        public Object getValue()
        {
            return value;
        }

        /**
         * Returns the value of the metric as a number.
         */
        public object getNumberValue()
        {
            return value;
        }

        /**
         * Returns true if the value for this metric is numeric.
         */
        public bool hasNumberValue()
        {
            return (value.IsNumber());
        }

        public override bool Equals(Object obj)
        {
            if (obj == null || !(obj is Metric))
            {
                return false;
            }
            Metric m = (Metric)obj;
            return config.Equals(m.getConfig())
                && timestamp == m.getTimestamp()
                && value.Equals(m.getValue());
        }


        public override int GetHashCode()
        {
            int result = config.GetHashCode();
            result = 31 * result + (int)(timestamp ^ ((long)((ulong)timestamp >> 32)));
            result = 31 * result + value.GetHashCode();
            return result;
        }


        public override string ToString()
        {
            return "Metric{config=" + config + ", timestamp=" + timestamp + ", value=" + value + '}';
        }
    }
}
