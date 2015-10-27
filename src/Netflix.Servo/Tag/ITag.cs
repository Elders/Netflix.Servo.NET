namespace Netflix.Servo.Tag
{
    /// <summary>
    /// A key-value pair associated with a metric.
    /// </summary>
    public interface ITag
    {
        /// <summary>
        /// Returns the key corresponding to this tag.
        /// </summary>
        string Key { get; }

        /// <summary>
        /// Returns the value corresponding to this tag.
        /// </summary>
        string Value { get; }
    }

    public enum DataSourceTypeEnum
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
    public class DataSourceType : ITag
    {
        private DataSourceType(string name)
        {
            this.name = name;
        }

        /**
         * A gauge is for numeric values that can be sampled without modification.
         * Examples of metrics that should be gauges are things like current
         * temperature, number of open connections, disk usage, etc.
         */
        public static DataSourceType GAUGE = new DataSourceType(nameof(DataSourceTypeEnum.GAUGE));

        /**
         * A counter is for numeric values that get incremented when some event
         * occurs. Counters will be sampled and converted into a rate of change
         * per second. Counter values should be monotonically increasing, i.e.,
         * the value should not decrease.
         */
        public static DataSourceType COUNTER = new DataSourceType(nameof(DataSourceTypeEnum.COUNTER));

        /**
         * A rate is for numeric values that represent a rate per second.
         */
        public static DataSourceType RATE = new DataSourceType(nameof(DataSourceTypeEnum.RATE));

        /**
         * A normalized rate per second. For counters that report values based on step
         * boundaries.
         */
        public static DataSourceType NORMALIZED = new DataSourceType(nameof(DataSourceTypeEnum.NORMALIZED));

        /**
         * An informational attribute is for values that might be useful for
         * debugging, but will not be collected as metrics for monitoring purposes.
         * These values are made available in JMX.
         */
        public static DataSourceType INFORMATIONAL = new DataSourceType(nameof(DataSourceTypeEnum.INFORMATIONAL));

        /**
         * Key name used for the data source type tag, configurable via
         * servo.datasourcetype.key system property.
         */
        //public static String KEY = System.getProperty("servo.datasourcetype.key", "type");
        //TODO: Fix this
        public static string KEY = "type";
        public string name = "default";

        public string Key { get { return KEY; } }

        public string Value { get { return name; } }

        public string tagString()
        {
            return Key + "=" + Value;
        }
    }

    public enum DataSourceLevelEnum
    {
        Debug,
        Info,
        Critical,
    }

    /**
 * Indicates a level for the monitor. This is meant to be similar to log levels to provide a
 * quick way to perform high-level filtering.
 */
    public class DataSourceLevel : ITag
    {
        private DataSourceLevel(string name)
        {
            this.name = name;
        }

        /**
         * Fine granularity monitors that provide a high amount of detail.
         */
        public static DataSourceLevel Debug = new DataSourceLevel(nameof(DataSourceLevelEnum.Debug));

        /**
         * The default level for monitors.
         */
        public static DataSourceLevel Info = new DataSourceLevel(nameof(DataSourceLevelEnum.Info));

        /**
         * Most important monitors for an application.
         */
        public static DataSourceLevel Critical = new DataSourceLevel(nameof(DataSourceLevelEnum.Critical));


        public static string KEY = "level";
        public string name = "default";

        public string Key { get { return KEY; } }

        public string Value { get { return name; } }

        public string tagString()
        {
            return Key + "=" + Value;
        }
    }
}
