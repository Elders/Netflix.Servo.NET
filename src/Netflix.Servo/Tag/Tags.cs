using System;
using System.Collections.Generic;

namespace Netflix.Servo.Tag
{
    /**
 * Helper functions for working with tags and tag lists.
 */
    public class Tags
    {
        /**
         * Keep track of the strings that have been used for keys and values.
         */
        private static HashSet<string> STR_CACHE = new HashSet<string>();

        /**
         * Keep track of tags that have been seen before and reuse.
         */
        private static HashSet<ITag> TAG_CACHE = new HashSet<ITag>();

        /**
         * Intern strings used for tag keys or values.
         */
        public static string intern(string v)
        {
            STR_CACHE.Add(v);
            return v;
        }

        /**
         * Returns the canonical representation of a tag.
         */
        public static ITag intern(ITag t)
        {
            TAG_CACHE.Add(t);
            return t;
        }

        /**
         * Interns custom tag types, assumes that basic tags are already interned. This is used to
         * ensure that we have a common view of tags internally. In particular, different subclasses of
         * Tag may not be equal even if they have the same key and value. Tag lists should use this to
         * ensure the equality will work as expected.
         */
        static ITag internCustom(ITag t)
        {
            return (t is BasicTag) ? t : newTag(t.getKey(), t.getValue());
        }

        /**
         * Create a new tag instance.
         */
        public static ITag newTag(string key, string value)
        {
            ITag newTag = new BasicTag(intern(key), intern(value));
            return intern(newTag);
        }

        /**
         * Parse a string representing a tag. A tag string should have the format {@code key=value}.
         * Whitespace at the ends of the key and value will be removed. Both the key and value must
         * have at least one character.
         *
         * @param tagString string with encoded tag
         * @return tag parsed from the string
         */
        public static ITag parseTag(string tagString)
        {
            string k;
            string v;
            int eqIndex = tagString.IndexOf("=", StringComparison.Ordinal);

            if (eqIndex < 0)
            {
                throw new InvalidOperationException("key and value must be separated by '='");
            }

            k = tagString.Substring(0, eqIndex).Trim();
            v = tagString.Substring(eqIndex + 1, tagString.Length).Trim();
            return newTag(k, v);
        }

        /**
         * Utility class.
         */
        private Tags()
        {
        }
    }
}
