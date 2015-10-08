using System;
using Netflix.Servo.Util;

namespace Netflix.Servo.Tag
{
    /// <summary>
    /// Immutable tag.
    /// </summary>
    public class BasicTag : ITag
    {
        private readonly string key;
        private readonly string value;

        /// <summary>
        /// Creates a new instance with the specified key and value.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public BasicTag(string key, string value)
        {
            this.key = checkNotEmpty(key, "key");
            this.value = checkNotEmpty(value, "value");
        }

        /// <summary>
        /// Verify that the {@code v} is not null or an empty string.
        /// </summary>
        static string checkNotEmpty(string v, string name)
        {
            Preconditions.checkNotNull(v, name);
            Preconditions.checkArgument(!string.IsNullOrEmpty(v), name + " cannot be empty");
            return v;
        }

        public string getKey()
        {
            return key;
        }


        public string getValue()
        {
            return value;
        }

        public override bool Equals(Object obj)
        {
            if (this == obj)
            {
                return true;
            }
            else if (obj is ITag)
            {
                ITag t = (ITag)obj;
                return key.Equals(t.getKey()) && value.Equals(t.getValue());
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            int result = key.GetHashCode();
            result = 31 * result + value.GetHashCode();
            return result;
        }

        public override string ToString()
        {
            return key + "=" + value;
        }



        /// <summary>
        /// Parse a string representing a tag. A tag string should have the format {@code key=value}.
        ///Whitespace at the ends of the key and value will be removed.Both the key and value must
        ///have at least one character.
        ///@param tagString string with encoded tag
        ///@return tag parsed from the string
        ///@deprecated Use Tags.parseTag instead.
        /// </summary>
        /// <param name="tagString"></param>
        /// <returns></returns>
        public static BasicTag parseTag(string tagString)
        {
            return (BasicTag)Tags.parseTag(tagString);
        }

        public string tagString()
        {
            return ToString();
        }
    }
}
