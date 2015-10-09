using System;

namespace Netflix.Servo.Tag
{
    /**
 * Standard tag keys that are used within this library.
 */
    public class StandardTagKeys
    {
        /**
         * Canonical name for the class that is providing the metric.
         */
        public static StandardTagKeys CLASS_NAME = new StandardTagKeys("ClassName");

        /**
         * Monitor id if one is provided via the annotation.
         */
        public static StandardTagKeys MONITOR_ID = new StandardTagKeys("MonitorId");

        private String keyName;

        StandardTagKeys(String keyName)
        {
            this.keyName = keyName;
        }

        public String getKeyName()
        {
            return keyName;
        }
    }
}
