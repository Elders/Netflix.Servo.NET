using System;
using System.Collections.Generic;
using System.Linq;
using Java.Util.Concurrent.Atomic;
using Netflix.Servo.Tag;

namespace Netflix.Servo.Monitor
{

    /**
 * Configuration settings associated with a monitor. A config consists of a name that is required
 * and an optional set of tags.
 */
    public sealed class MonitorConfig
    {

        /**
         * A builder to assist in creating monitor config objects.
         */
        public class Builder
        {
            public string name;
            public SmallTagMap.Builder tagsBuilder = SmallTagMap.builder();
            public IPublishingPolicy policy = DefaultPublishingPolicy.getInstance();

            /**
             * Create a new builder initialized with the specified config.
             */
            public Builder(MonitorConfig config) : this(config.getName())
            {
                withTags(config.getTags());
                withPublishingPolicy(config.getPublishingPolicy());
            }

            /**
             * Create a new builder initialized with the specified name.
             */
            public Builder(string name)
            {
                this.name = name;
            }

            /**
             * Add a tag to the config.
             */
            public Builder withTag(string key, string val)
            {
                tagsBuilder.add(Tags.newTag(key, val));
                return this;
            }

            /**
             * Add a tag to the config.
             */
            public Builder withTag(ITag tag)
            {
                tagsBuilder.add(tag);
                return this;
            }

            /**
             * Add all tags in the list to the config.
             */
            public Builder withTags(ITagList tagList)
            {
                if (tagList != null)
                {
                    foreach (var t in tagList)
                    {
                        tagsBuilder.add(t);
                    }
                }
                return this;
            }

            /**
             * Add all tags in the list to the config.
             */
            public Builder withTags(ICollection<ITag> tagCollection)
            {
                tagsBuilder.addAll(tagCollection);
                return this;
            }

            /**
             * Add all tags from a given SmallTagMap.
             */
            public Builder withTags(SmallTagMap.Builder tagsBuilder)
            {
                this.tagsBuilder = tagsBuilder;
                return this;
            }

            /**
             * Add the publishing policy to the config.
             */
            public Builder withPublishingPolicy(IPublishingPolicy policy)
            {
                this.policy = policy;
                return this;
            }

            /**
             * Create the monitor config object.
             */
            public MonitorConfig build()
            {
                return new MonitorConfig(this);
            }

            /**
             * Get the name for this monitor config.
             */
            public string getName()
            {
                return name;
            }

            /**
             * Get the list of tags for this monitor config.
             */
            public IEnumerable<ITag> getTags()
            {
                return tagsBuilder.result().ToList().AsReadOnly();
            }

            /**
             * Get the publishingPolicy.
             */
            public IPublishingPolicy getPublishingPolicy()
            {
                return policy;
            }
        }

        /**
         * Return a builder instance with the specified name.
         */
        public static Builder builder(string name)
        {
            return new Builder(name);
        }

        private string name;
        private ITagList tags;
        private IPublishingPolicy policy;

        /**
         * Config is immutable, cache the hash code to improve performance.
         */
        private AtomicInteger cachedHashCode = new AtomicInteger(0);

        /**
         * Creates a new instance with a given name and tags. If {@code tags} is
         * null an empty tag list will be used.
         */
        private MonitorConfig(Builder builder)
        {
            this.name = builder.name;
            this.tags = (builder.tagsBuilder.isEmpty())
                ? BasicTagList.EMPTY
                : new BasicTagList(builder.tagsBuilder.result());
            this.policy = builder.policy;
        }

        /**
         * Returns the name of the metric.
         */
        public string getName()
        {
            return name;
        }

        /**
         * Returns the tags associated with the metric.
         */
        public ITagList getTags()
        {
            return tags;
        }

        /**
         * Returns the publishing policy.
         */
        public IPublishingPolicy getPublishingPolicy()
        {
            return policy;
        }


        public override bool Equals(Object obj)
        {
            if (this == obj)
            {
                return true;
            }
            if (obj == null || !(obj is MonitorConfig))
            {
                return false;
            }
            MonitorConfig m = (MonitorConfig)obj;
            return name.Equals(m.getName())
                && tags.Equals(m.getTags())
                && policy.Equals(m.getPublishingPolicy());
        }

        /**
         * This class is immutable so we cache the hash code after the first time it is computed. The
         * value 0 is used as an indicator that the hash code has not yet been computed, this means the
         * cache won't work for a small set of inputs, but the impact should be minimal for a decent
         * hash function. Similar technique is used for java String class.
         */
        public override int GetHashCode()
        {
            var hash = name.GetHashCode();
            hash = 31 * hash + tags.GetHashCode();
            hash = 31 * hash + policy.GetHashCode();
            return hash;
        }

        public override string ToString()
        {
            return "MonitorConfig{name=" + name + ", tags=" + tags + ", policy=" + policy + '}';
        }

        /**
         * Returns a copy of the current MonitorConfig.
         */
        private MonitorConfig.Builder copy()
        {
            return MonitorConfig.builder(name).withTags(tags).withPublishingPolicy(policy);
        }

        /**
         * Returns a copy of the monitor config with an additional tag.
         */
        public MonitorConfig withAdditionalTag(ITag tag)
        {
            return copy().withTag(tag).build();
        }

        /**
         * Returns a copy of the monitor config with additional tags.
         */
        public MonitorConfig withAdditionalTags(ITagList newTags)
        {
            return copy().withTags(newTags).build();
        }
    }
}