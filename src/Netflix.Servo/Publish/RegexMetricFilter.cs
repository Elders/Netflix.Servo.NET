using System;
using System.Text.RegularExpressions;
using Netflix.Servo.Monitor;
using Netflix.Servo.Tag;

namespace Netflix.Servo.Publish
{
    /**
 * Filter that checks if a tag value matches a regular expression.
 */
    public class RegexMetricFilter : MetricFilter
    {

        private String tagKey;
        private Regex pattern;
        private bool matchIfMissingTag;
        private bool invert;

        /**
         * Creates a new regex filter.
         *
         * @param tagKey            tag to check against the pattern
         * @param pattern           pattern to check
         * @param matchIfMissingTag should metrics without the specified tag match?
         * @param invert            should the match be inverted?
         */
        public RegexMetricFilter(
            String tagKey,
            Regex pattern,
            bool matchIfMissingTag,
            bool invert)
        {
            this.tagKey = tagKey;
            this.pattern = pattern;
            this.matchIfMissingTag = matchIfMissingTag;
            this.invert = invert;
        }

        /**
         * {@inheritDoc}
         */
        public bool matches(MonitorConfig config)
        {
            String name = config.getName();
            ITagList tags = config.getTags();
            String value;
            if (tagKey == null)
            {
                value = name;
            }
            else
            {
                ITag t = tags.getTag(tagKey);
                value = (t == null) ? null : t.getValue();
            }

            bool match = matchIfMissingTag;
            if (value != null)
            {
                match = pattern.Match(value).Success;
            }
            return match ^ invert;
        }
    }
}
