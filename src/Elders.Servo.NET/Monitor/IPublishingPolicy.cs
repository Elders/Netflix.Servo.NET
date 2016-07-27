namespace Elders.Servo.NET.Monitor
{
    public interface IPublishingPolicy
    {
    }

    /**
* The default publishing policy. Observers must follow the default behaviour when the
* {@link MonitorConfig} associated with a {@link Monitor} uses this policy.
*/
    public sealed class DefaultPublishingPolicy : IPublishingPolicy
    {
        private static DefaultPublishingPolicy INSTANCE = new DefaultPublishingPolicy();

        private DefaultPublishingPolicy()
        {
        }

        public static DefaultPublishingPolicy getInstance()
        {
            return INSTANCE;
        }


        public override string ToString()
        {
            return "DefaultPublishingPolicy";
        }
    }
}
