using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Elders.Servo.NET.Tag
{

    /**
 * Immutable tag list.
 */
    public class BasicTagList : ITagList
    {
        /**
         * An empty tag list.
         */
        public static ITagList EMPTY = new BasicTagList(Enumerable.Empty<ITag>());

        private SmallTagMap tagMap;
        private SortedDictionary<String, String> sortedTaglist;

        /**
         * Create a BasicTagList from a {@link SmallTagMap}.
         */
        public BasicTagList(SmallTagMap tagMap)
        {
            this.tagMap = tagMap;
            this.sortedTaglist = new SortedDictionary<string, string>();
        }

        /**
         * Creates a new instance with a fixed set of tags.
         *
         * @param entries entries to include in this tag list
         */
        public BasicTagList(IEnumerable<ITag> entries)
        {
            SmallTagMap.Builder builder = SmallTagMap.builder();
            builder.addAll(entries);
            tagMap = builder.result();
        }

        public ITag getTag(String key)
        {
            return tagMap.get(key);
        }

        public String getValue(String key)
        {
            ITag t = tagMap.get(key);
            return (t == null) ? null : t.Value;
        }

        public bool containsKey(String key)
        {
            return tagMap.containsKey(key);
        }

        public bool isEmpty()
        {
            return tagMap.isEmpty();
        }

        public int size()
        {
            return tagMap.size();
        }

        public IEnumerator<ITag> GetEnumerator()
        {
            return tagMap.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /**
         * {@inheritDoc}
         */
        public IDictionary<String, String> asMap()
        {
            if (sortedTaglist != null)
            {
                return sortedTaglist;
            }

            SortedDictionary<String, String> tm = new SortedDictionary<String, String>();
            foreach (var tag in this.tagMap)
            {
                tm.Add(tag.Key, tag.Value);
            }

            sortedTaglist = new SortedDictionary<string, string>(tm);
            return sortedTaglist;
        }

        /**
         * Returns a new tag list with additional tags from {@code tags}. If there
         * is a conflict with tag keys the tag from {@code tags} will be used.
         */
        public BasicTagList copy(ITagList tags)
        {
            return concat(this, tags);
        }

        /**
         * Returns a new tag list with an additional tag. If {@code key} is
         * already present in this tag list the value will be overwritten with
         * {@code value}.
         */
        public BasicTagList copy(String key, String value)
        {
            return concat(this, Tags.newTag(key, value));
        }

        public override bool Equals(Object obj)
        {
            return this == obj
                || (obj is BasicTagList) && tagMap.equals(((BasicTagList)obj).tagMap);
        }

        public override int GetHashCode()
        {
            return tagMap.GetHashCode();
        }

        public override String ToString()
        {
            return String.Join(",", tagMap);
        }

        /**
         * Returns a tag list containing the union of {@code t1} and {@code t2}.
         * If there is a conflict with tag keys, the tag from {@code t2} will be
         * used.
         */
        public static BasicTagList concat(ITagList t1, ITagList t2)
        {
            return new BasicTagList(t1.Concat(t2));
        }

        /**
         * Returns a tag list containing the union of {@code t1} and {@code t2}.
         * If there is a conflict with tag keys, the tag from {@code t2} will be
         * used.
         */
        public static BasicTagList concat(ITagList t1, params ITag[] t2)
        {
            return new BasicTagList(t1.Concat(t2));
        }

        /**
         * Returns a tag list from the list of key values passed.
         * <p/>
         * Example:
         * <p/>
         * <code>
         * BasicTagList tagList = BasicTagList.of("id", "someId", "class", "someClass");
         * </code>
         */
        public static BasicTagList of(params string[] tags)
        {
            //Preconditions.checkArgument(tags.Length % 2 == 0, "tags must be a sequence of key,value pairs");

            SmallTagMap.Builder builder = SmallTagMap.builder();
            for (int i = 0; i < tags.Length; i += 2)
            {
                ITag t = Tags.newTag(tags[i], tags[i + 1]);
                builder.add(t);
            }
            return new BasicTagList(builder.result());
        }

        /**
         * Returns a tag list from the tags.
         */
        public static BasicTagList of(params ITag[] tags)
        {
            return new BasicTagList(tags);
        }

        /**
         * Returns a tag list that has a copy of {@code tags}.
         *
         * @deprecated Use {@link #of(Tag...)}
         */
        [Obsolete]
        public static BasicTagList copyOf(params ITag[] tags)
        {
            return new BasicTagList(tags);
        }

        /**
         * Returns a tag list that has a copy of {@code tags}. Each tag value
         * is expected to be a string parseable using {@link BasicTag#parseTag}.
         *
         * @deprecated Use {@link #of(String...)} with separate key, values instead.
         */
        [Obsolete]
        public static BasicTagList copyOf(params String[] tags)
        {
            return copyOf(tags);
        }

        /**
         * Returns a tag list that has a copy of {@code tags}. Each tag value
         * is expected to be a string parseable using {@link BasicTag#parseTag}.
         */
        public static BasicTagList copyOf(IEnumerable<String> tags)
        {
            SmallTagMap.Builder builder = SmallTagMap.builder();
            foreach (var tag in tags)
            {
                builder.add(Tags.parseTag(tag));
            }
            return new BasicTagList(builder.result());
        }

        /**
         * Returns a tag list that has a copy of {@code tags}.
         */
        public static BasicTagList copyOf(IDictionary<String, String> tags)
        {
            SmallTagMap.Builder builder = SmallTagMap.builder();
            foreach (var tag in tags)
            {
                builder.add(Tags.newTag(tag.Key, tag.Value));
            }

            return new BasicTagList(builder.result());
        }

        public IDictionary<string, string> AsDictionary()
        {
            throw new NotImplementedException();
        }
    }
}