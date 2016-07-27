using System;

namespace Elders.Servo.NET.Attributes
{
    /**
     * Tags to associate with all metrics in an instance. The tags will be queried
     * when the instance is registered with the {@link
     * com.Elders.Servo.NET.MonitorRegistry} and used to provide a common base set of
     * tags for all attribute annotated with {@link Monitor}. Tags provided on the
     * {@link Monitor} annotation will override these tags if there is a common
     * key.
     */
    //@Documented
    //@Retention(RetentionPolicy.RUNTIME)
    //@Target({ ElementType.FIELD, ElementType.METHOD })
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Method)]
    public class MonitorTagsAttribute : Attribute
    {
    }
}
