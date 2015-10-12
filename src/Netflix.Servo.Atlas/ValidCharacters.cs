using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Netflix.Servo.Monitor;
using Netflix.Servo.Tag;

namespace Netflix.Servo.Atlas
{
    /**
 * Utility class to deal with rewriting keys/values to the character set accepted by atlas.
 */
    public class ValidCharacters
    {
        /**
         * Only allow letters, numbers, underscores, dashes and dots in our identifiers.
         */

        private static Dictionary<char, bool> CHARS_ALLOWED = new Dictionary<char, bool>();

        static ValidCharacters()
        {
            CHARS_ALLOWED['.'] = true;
            CHARS_ALLOWED['-'] = true;
            CHARS_ALLOWED['_'] = true;
            for (char ch = '0'; ch <= '9'; ch++)
            {
                CHARS_ALLOWED[ch] = true;
            }
            for (char ch = 'A'; ch <= 'Z'; ch++)
            {
                CHARS_ALLOWED[ch] = true;
            }
            for (char ch = 'a'; ch <= 'z'; ch++)
            {
                CHARS_ALLOWED[ch] = true;
            }
        }

        private ValidCharacters()
        {
            // utility class
        }

        /**
         * Check whether a given string contains an invalid character.
         */
        public static bool hasInvalidCharacters(String str)
        {
            int n = str.Length;
            for (int i = 0; i < n; i++)
            {
                char c = str[i];
                if (c >= CHARS_ALLOWED.Count || !CHARS_ALLOWED[c])
                {
                    return true;
                }
            }
            return false;
        }

        /**
         * Convert a given string to one where all characters are valid.
         */
        public static String toValidCharset(String str)
        {
            int n = str.Length;
            StringBuilder buf = new StringBuilder(n + 1);
            for (int i = 0; i < n; i++)
            {
                char c = str[i];
                if (c < CHARS_ALLOWED.Count && CHARS_ALLOWED[c])
                {
                    buf.Append(c);
                }
                else
                {
                    buf.Append('_');
                }
            }
            return buf.ToString();
        }

        /**
         * Return a new metric where the name and all tags are using the valid character
         * set.
         */
        public static Metric toValidValue(Metric metric)
        {
            MonitorConfig cfg = metric.getConfig();
            MonitorConfig.Builder cfgBuilder = MonitorConfig.builder(toValidCharset(cfg.getName()));
            foreach (ITag orig in cfg.getTags())
            {
                cfgBuilder.withTag(toValidCharset(orig.getKey()), toValidCharset(orig.getValue()));
            }
            cfgBuilder.withPublishingPolicy(cfg.getPublishingPolicy());
            return new Metric(cfgBuilder.build(), metric.getTimestamp(), metric.getValue());
        }

        /**
         * Create a new list of metrics where all metrics are using the valid character set.
         */
        public static List<Metric> toValidValues(List<Metric> metrics)
        {
            return metrics.Select(x => toValidValue(x)).ToList();
        }
    }
}
