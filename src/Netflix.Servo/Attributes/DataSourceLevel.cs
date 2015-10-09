using System;
using Netflix.Servo.Tag;

namespace Netflix.Servo.Attributes
{
    public enum DataSourceLevel
    {
        DEBUG,
        INFO,
        CRITICAL
    }

    /**
 * Indicates a level for the monitor. This is meant to be similar to log levels to provide a
 * quick way to perform high-level filtering.
 */
    public class DataSourceLevelTag : ITag
    {
        private DataSourceLevelTag(string name)
        {
            this.name = name;
        }

        /**
         * Fine granularity monitors that provide a high amount of detail.
         */
        public static DataSourceLevelTag DEBUG = new DataSourceLevelTag(nameof(DataSourceLevel.DEBUG));

        /**
         * The default level for monitors.
         */
        public static DataSourceLevelTag INFO = new DataSourceLevelTag(nameof(DataSourceLevel.INFO));

        /**
         * Most important monitors for an application.
         */
        public static DataSourceLevelTag CRITICAL = new DataSourceLevelTag(nameof(DataSourceLevel.CRITICAL));

        /**
         * Key name used for the data source level tag.
         */
        public static String KEY = "level";
        public string name = "default";

        /**
         * {@inheritDoc}
         */
        public String getKey()
        {
            return KEY;
        }

        /**
         * {@inheritDoc}
         */
        public String getValue()
        {
            return name;
        }

        /**
         * {@inheritDoc}
         */
        public String tagString()
        {
            return getKey() + "=" + getValue();
        }
    }
}
