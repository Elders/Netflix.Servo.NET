using System;
using Netflix.Servo.Tag;

namespace Netflix.Servo.Attributes
{
    public enum DataSourceType
    {
        GAUGE,
        COUNTER,
        RATE,
        NORMALIZED,
        INFORMATIONAL
    }

    /**
 * Indicates the type of value that is annotated to determine how it will be
 * measured.
 */
    public class DataSourceTypeTag : ITag
    {
        private DataSourceTypeTag(string name)
        {
            this.name = name;
        }

        /**
         * A gauge is for numeric values that can be sampled without modification.
         * Examples of metrics that should be gauges are things like current
         * temperature, number of open connections, disk usage, etc.
         */
        public static DataSourceTypeTag GAUGE = new DataSourceTypeTag(nameof(DataSourceType.GAUGE));

        /**
         * A counter is for numeric values that get incremented when some event
         * occurs. Counters will be sampled and converted into a rate of change
         * per second. Counter values should be monotonically increasing, i.e.,
         * the value should not decrease.
         */
        public static DataSourceTypeTag COUNTER = new DataSourceTypeTag(nameof(DataSourceType.COUNTER));

        /**
         * A rate is for numeric values that represent a rate per second.
         */
        public static DataSourceTypeTag RATE = new DataSourceTypeTag(nameof(DataSourceType.RATE));

        /**
         * A normalized rate per second. For counters that report values based on step
         * boundaries.
         */
        public static DataSourceTypeTag NORMALIZED = new DataSourceTypeTag(nameof(DataSourceType.NORMALIZED));

        /**
         * An informational attribute is for values that might be useful for
         * debugging, but will not be collected as metrics for monitoring purposes.
         * These values are made available in JMX.
         */
        public static DataSourceTypeTag INFORMATIONAL = new DataSourceTypeTag(nameof(DataSourceType.INFORMATIONAL));

        /**
         * Key name used for the data source type tag, configurable via
         * servo.datasourcetype.key system property.
         */
        //public static String KEY = System.getProperty("servo.datasourcetype.key", "type");
        //TODO: Fix this
        public static String KEY = "";
        public string name = "default";

        public String getKey()
        {
            return KEY;
        }

        public String getValue()
        {
            return name;
        }

        public String tagString()
        {
            return getKey() + "=" + getValue();
        }
    }
}