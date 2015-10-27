namespace Netflix.Servo.Tag
{
    public class NameTag : BasicTag
    {
        public const string KeyValue = "name";

        public NameTag(string value) : base(KeyValue, value) { }
    }
}
