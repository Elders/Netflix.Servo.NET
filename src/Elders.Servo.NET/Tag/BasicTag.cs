using Elders.Servo.NET.Util;

namespace Elders.Servo.NET.Tag
{
    /// <summary>
    /// Immutable tag.
    /// </summary>
    public class BasicTag : ValueObject<BasicTag>, ITag
    {
        /// <summary>
        /// Creates a new instance with the specified key and value.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public BasicTag(string key, string value)
        {
            Key = key;
            Value = value;
        }

        public string Key { get; }

        public string Value { get; }

        public override string ToString()
        {
            return Key + "=" + Value;
        }
    }
}
