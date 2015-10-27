using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Netflix.Servo.Tag
{

    /// <summary>
    /// This class is not intended to be used by 3rd parties and should be considered an implementation detail.
    /// </summary>
    public class SmallTagMap : IEnumerable<ITag>
    {
        /// <summary>
        /// Max number of tags supported in a tag map. Attempting to add additional tags will result in a warning logged.
        /// </summary>
        public const int MAX_TAGS = 32;

        /// <summary>
        /// Initial size for the map.
        /// </summary>
        public const int INITIAL_TAG_SIZE = 8;

        //private static ILogger LOGGER = LoggerFactory.GetLogger(typeof(SmallTagMap));

        /// <summary>
        /// Return a new builder to assist in creating a new SmallTagMap using the default tag size (8).
        /// </summary>
        /// <returns></returns>
        public static Builder builder()
        {
            return new Builder(INITIAL_TAG_SIZE);
        }

        /// <summary>
        /// Helper class to build the immutable map.
        /// </summary>
        public class Builder
        {
            private int actualSize = 0;
            private int size;
            private Object[] buf;

            private void init(int size)
            {
                this.size = size;
                buf = new Object[size * 2];
                actualSize = 0;
            }

            /**
             * Create a new builder with the specified capacity.
             *
             * @param size Size of the underlying array.
             */
            public Builder(int size)
            {
                init(size);
            }

            /**
             * Get the number of entries in this map..
             */
            public int Size()
            {
                return actualSize;
            }

            /**
             * True if this builder does not have any tags added to it.
             */
            public bool isEmpty()
            {
                return actualSize == 0;
            }

            private void resizeIfPossible(ITag tag)
            {
                if (size < MAX_TAGS)
                {
                    Object[] prevBuf = buf;
                    init(size * 2);
                    for (int i = 1; i < prevBuf.Length; i += 2)
                    {
                        ITag t = (ITag)prevBuf[i];
                        if (t != null)
                        {
                            add(t);
                        }
                    }
                    add(tag);
                }
                else
                {
                    string msg = $"Cannot add Tag {tag} - Maximum number of tags ({MAX_TAGS}) reached.";
                    //LOGGER.Error(msg);
                }
            }

            /**
             * Adds a new tag to this builder.
             */
            public Builder add(ITag tag)
            {
                string k = tag.Key;
                int pos = (int)(Math.Abs((long)k.GetHashCode()) % size);
                int i = pos;
                Object ki = buf[i * 2];
                while (ki != null && !ki.Equals(k))
                {
                    i = (i + 1) % size;
                    if (i == pos)
                    {
                        resizeIfPossible(tag);
                        return this;
                    }
                    ki = buf[i * 2];
                }

                if (ki != null)
                {
                    buf[i * 2] = k;
                    buf[i * 2 + 1] = tag;
                }
                else
                {
                    if (buf[i * 2] != null)
                    {
                        throw new InvalidOperationException("position has already been filled");
                    }
                    buf[i * 2] = k;
                    buf[i * 2 + 1] = tag;
                    actualSize += 1;
                }
                return this;
            }

            /**
             * Adds all tags from the {@link Iterable} tags to this builder.
             */
            public Builder addAll(IEnumerable<ITag> tags)
            {
                foreach (var tag in tags)
                {
                    add(tag);
                }
                return this;
            }

            /**
             * Get the resulting SmallTagMap.
             */
            public SmallTagMap result()
            {
                ITag[] tagArray = new ITag[actualSize];
                int tagIdx = 0;

                for (int i = 1; i < buf.Length; i += 2)
                {
                    Object o = buf[i];
                    if (o != null)
                    {
                        tagArray[tagIdx++] = (ITag)o;
                    }
                }
                Array.Sort(tagArray, (o1, o2) =>
                {
                    int keyCmp = string.Compare(o1.Key, o2.Key, StringComparison.Ordinal);
                    if (keyCmp != 0)
                    {
                        return keyCmp;
                    }
                    return string.Compare(o1.Value, o2.Value, StringComparison.Ordinal);
                });
                Debug.Assert(tagIdx == actualSize);
                return new SmallTagMap(tagArray);
            }
        }

        public IEnumerator<ITag> GetEnumerator()
        {
            return tagArray.GetEnumerator();
        }

        private readonly List<ITag> tagArray;

        /**
         * Create a new SmallTagMap using the given array and size.
         *
         * @param tagArray sorted array of tags
         */
        SmallTagMap(ITag[] tagArray)
        {
            if (tagArray == null)
                throw new ArgumentNullException(nameof(tagArray));

            this.tagArray = new List<ITag>(tagArray);
        }

        static int binarySearch(ITag[] a, string key)
        {
            int low = 0;
            int high = a.Length - 1;

            while (low <= high)
            {
                int mid = (int)(((uint)(low + high)) >> 1);
                ITag midValTag = a[mid];
                string midVal = midValTag.Key;
                int cmp = string.Compare(midVal, key, StringComparison.Ordinal);
                if (cmp < 0)
                {
                    low = mid + 1;
                }
                else if (cmp > 0)
                {
                    high = mid - 1;
                }
                else
                {
                    return mid; // tag key found
                }
            }
            return -(low + 1);  // tag key not found.
        }

        /**
         * Get the tag associated with a given key.
         */
        public ITag get(string key)
        {
            int idx = binarySearch(tagArray.ToArray(), key);
            if (idx < 0)
            {
                return null;
            }
            else
            {
                return tagArray[idx];
            }
        }

        /**
         * {@inheritDoc}
         */

        public override int GetHashCode()
        {
            return tagArray.GetHashCode();
        }

        /**
         * {@inheritDoc}
         */

        public override string ToString()
        {
            return "SmallTagMap{" + string.Join(",", GetEnumerator()) + "}";
        }

        /**
         * Returns true whether this map contains a Tag with the given key.
         */
        public bool containsKey(string k)
        {
            return get(k) != null;
        }

        /**
         * Returns true if this map has no entries.
         */
        public bool isEmpty()
        {
            return tagArray.Count == 0;
        }

        /**
         * Returns the number of Tags stored in this map.
         */
        public int size()
        {
            return tagArray.Count;
        }

        /**
         * Returns the {@link Set} of tags.
         *
         * @deprecated This method will be removed in the next version. This is an expensive method
         * and not in the spirit of this class which is to be more efficient than the standard
         * collections library.
         */
        [Obsolete]
        public ISet<ITag> tagSet()
        {
            return new HashSet<ITag>(tagArray);
        }


        /** {@inheritDoc} */
        public bool equals(Object obj)
        {
            if (this == obj)
            {
                return true;
            }

            if (obj == null || !(obj is SmallTagMap))
            {
                return false;
            }

            SmallTagMap that = (SmallTagMap)obj;
            return Equals(tagArray, that.tagArray);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}