using System;
using Elders.Servo.NET.Tag;

namespace Elders.Servo.NET.Attributes
{
    public enum DataSourceLevelEnum
    {
        DEBUG,
        INFO,
        CRITICAL
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
        public static DataSourceLevel DEBUG = new DataSourceLevel(nameof(DataSourceLevelEnum.DEBUG));

        /**
         * The default level for monitors.
         */
        public static DataSourceLevel INFO = new DataSourceLevel(nameof(DataSourceLevelEnum.INFO));

        /**
         * Most important monitors for an application.
         */
        public static DataSourceLevel CRITICAL = new DataSourceLevel(nameof(DataSourceLevelEnum.CRITICAL));

        /**
         * Key name used for the data source level tag.
         */
        public static String KEY = "level";
        public string name = "default";

        public string Key { get { return KEY; } }

        public string Value { get { return name; } }

        public String tagString()
        {
            return Key + "=" + Value;
        }
    }
}
