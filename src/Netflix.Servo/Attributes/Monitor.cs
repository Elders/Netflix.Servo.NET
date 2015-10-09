using System;

namespace Netflix.Servo.Attributes
{
    /**
 * Annotation indicating a field or method should be collected for monitoring.
 * The attributes annotated should be thread-safe for access by a background
 * thread. If a method is annotated it should be inexpensive and avoid any
 * potentially costly operations such as IO and networking. Expect that the
 * fields will be polled frequently and cache values that require expensive
 * computation rather than computing them inline.
 */
    //@Documented
    //@Retention(RetentionPolicy.RUNTIME)
    //@Target({ ElementType.FIELD, ElementType.METHOD })
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Method)]
    public class MonitorAttribute : Attribute
    {
        /**
         * Name of the annotated attribute.
         */
        public String name { get; set; } = default(string);

        /**
         * Type of value that is annotated, for more information see
         * {@link DataSourceType}.
         */
        public DataSourceTypeEnum type { get; set; } = DataSourceTypeEnum.INFORMATIONAL;

        /**
         * Level of the value that is annotated, for more information see
         * {@link DataSourceLevel}.
         */
        public DataSourceLevelEnum level { get; set; } = DataSourceLevelEnum.INFO;

        /**
         * A human readable description of the annotated attribute.
         */
        public String description { get; set; } = default(string);
    }
}
