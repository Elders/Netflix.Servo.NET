namespace Netflix.Servo.Monitor
{
    /**
 * The default publishing policy. Observers must follow the default behaviour when the
 * {@link MonitorConfig} associated with a {@link Monitor} uses this policy.
 */
    public sealed class DefaultPublishingPolicy : PublishingPolicy
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
