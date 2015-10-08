using System.Collections.Generic;

namespace Netflix.Servo.Tag
{
    /**
 * Represents a list of tags associated with a metric value.
 */
    public interface TagList : IEnumerable<ITag>
    {
        /**
         * Returns the tag matching a given key or null if not match is found.
         */
        ITag getTag(string key);

        /**
         * Returns the value matching a given key or null if not match is found.
         */
        string getValue(string key);

        /**
         * Returns true if this list has a tag with the given key.
         */
        bool containsKey(string key);

        /**
         * Returns true if this list is emtpy.
         */
        bool isEmpty();

        /**
         * Returns the number of tags in this list.
         */
        int size();

        /**
         * Returns a map containing a copy of the tags in this list.
         */
        IDictionary<string, string> asMap();
    }
}
