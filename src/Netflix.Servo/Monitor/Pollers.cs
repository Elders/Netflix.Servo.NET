using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using slf4net;

namespace Netflix.Servo.Monitor
{
    /**
 * Poller configuration. This class provides the mechanism
 * to know how many pollers will be used, and at their estimated polling intervals.
 */
    public class Pollers
    {
        private Pollers()
        {
        }

        /**
         * A comma separated list of longs indicating the frequency of the pollers. For example: <br/>
         * {@code 60000, 10000 }<br/>
         * indicates that the main poller runs every 60s and a secondary
         * poller will run every 10 seconds.
         * This is used to deal with monitors that need to get reset after they're polled.
         * For example a MinGauge or a MaxGauge.
         */
        public static String POLLERS = @"System.getProperty(""servo.pollers"", ""60000,10000"")";
        static long[] DEFAULT_PERIODS = { 60000L, 10000L };

        /**
         * Polling intervals in milliseconds.
         */
        public static long[] POLLING_INTERVALS = parse(POLLERS);

        private static List<long> POLLING_INTERVALS_AS_LIST;

        /**
         * Get list of polling intervals in milliseconds.
         */
        public static List<long> getPollingIntervals()
        {
            return POLLING_INTERVALS_AS_LIST;
        }

        /**
         * Number of pollers that will run.
         */
        public static int NUM_POLLERS = POLLING_INTERVALS.Length;

        /**
         * For debugging. Simple toString for non-empty arrays
         */
        private static String join(long[] a)
        {
            Debug.Assert(a.Length > 0);
            StringBuilder builder = new StringBuilder();
            builder.Append(a[0]);
            for (int i = 1; i < a.Length; ++i)
            {
                builder.Append(',');
                builder.Append(a[i]);
            }
            return builder.ToString();
        }

        /**
         * Parse the content of the system property that describes the polling intervals,
         * and in case of errors
         * use the default of one poller running every minute.
         */
        static long[] parse(String pollers)
        {
            String[] periods = pollers.Split(new string[] { ",\\s*" }, StringSplitOptions.None);
            long[] result = new long[periods.Length];

            bool errors = false;
            ILogger logger = LoggerFactory.GetLogger(typeof(Pollers));
            for (int i = 0; i < periods.Length; ++i)
            {
                String period = periods[i];
                try
                {
                    result[i] = long.Parse(period);
                    if (result[i] <= 0)
                    {
                        logger.Error("Invalid polling interval: {0} must be positive.", period);
                        errors = true;
                    }
                }
                catch (FormatException e)
                {
                    logger.Error("Cannot parse '{0}' as a long: {1}", period, e.Message);
                    errors = true;
                }
            }

            if (errors || periods.Length == 0)
            {
                logger.Info("Using a default configuration for poller intervals: {0}", join(DEFAULT_PERIODS));
                return DEFAULT_PERIODS;
            }
            else
            {
                return result;
            }
        }

        static Pollers()
        {
            List<long> intervals = new List<long>(POLLING_INTERVALS.Length);
            foreach (var interval in POLLING_INTERVALS)
            {
                intervals.Add(interval);
            }
            POLLING_INTERVALS_AS_LIST = intervals;
        }
    }
}
