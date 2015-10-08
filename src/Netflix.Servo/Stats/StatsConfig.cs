using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Netflix.Servo.Stats
{
    /**
 * Configuration options for a {@link com.netflix.servo.monitor.StatsTimer}
 * <p/>
 * By default we publish count (number of times the timer was executed), totalTime, and
 * 95.0, and 99.0 percentiles.
 * <p/>
 * The size for the buffer used to store samples is controlled using the sampleSize field,
 * and the frequency
 * at which stats are computed is controlled with the computeFrequencyMillis option.
 * By default these are
 * set to 100,000 entries in the buffer, and computation at 60,000 ms (1 minute) intervals.
 */
    public class StatsConfig
    {
        private static String CLASS_NAME = typeof(StatsConfig).FullName;
        private static String SIZE_PROP = CLASS_NAME + ".sampleSize";
        private static String FREQ_PROP = CLASS_NAME + ".computeFreqMillis";

        /**
         * Builder for StatsConfig. By default the configuration includes count,
         * total and 95th and 99th percentiles.
         */
        public class Builder
        {
            public bool publishCount = true;
            public bool publishTotal = true;
            public bool publishMin = false;
            public bool publishMax = false;
            public bool publishMean = false;
            public bool publishVariance = false;
            public bool publishStdDev = false;
            //public int sampleSize = int.Parse(System.getProperty(SIZE_PROP, "1000"));
            //public long frequencyMillis = long.Parse(System.getProperty(FREQ_PROP, "60000"));
            //TODO: Fix this
            public int sampleSize;
            public long frequencyMillis;

            public double[] percentiles = { 95.0, 99.0 };

            /**
             * Whether to publish count or not.
             */
            public Builder withPublishCount(bool publishCount)
            {
                this.publishCount = publishCount;
                return this;
            }

            /**
             * Whether to publish total or not.
             */
            public Builder withPublishTotal(bool publishTotal)
            {
                this.publishTotal = publishTotal;
                return this;
            }

            /**
             * Whether to publish min or not.
             */
            public Builder withPublishMin(bool publishMin)
            {
                this.publishMin = publishMin;
                return this;
            }

            /**
             * Whether to publish max or not.
             */
            public Builder withPublishMax(bool publishMax)
            {
                this.publishMax = publishMax;
                return this;
            }

            /**
             * Whether to publish an average statistic or not. Note that if you plan
             * to aggregate the values reported (for example across a cluster of nodes) you probably do
             * not want to publish the average per node, and instead want to compute it by publishing
             * total and count.
             */
            public Builder withPublishMean(bool publishMean)
            {
                this.publishMean = publishMean;
                return this;
            }

            /**
             * Whether to publish variance or not.
             */
            public Builder withPublishVariance(bool publishVariance)
            {
                this.publishVariance = publishVariance;
                return this;
            }


            /**
             * Whether to publish standard deviation or not.
             */
            public Builder withPublishStdDev(bool publishStdDev)
            {
                this.publishStdDev = publishStdDev;
                return this;
            }

            /**
             * Set the percentiles to compute.
             *
             * @param percentiles An array of doubles describing which percentiles to compute. For
             *                    example {@code {95.0, 99.0}}
             */
            public Builder withPercentiles(double[] percentiles)
            {
                Array.Copy(percentiles, this.percentiles, percentiles.Length);
                return this;
            }

            /**
             * Set the sample size.
             */
            public Builder withSampleSize(int size)
            {
                this.sampleSize = size;
                return this;
            }

            /**
             * How often to compute the statistics. Usually this will be set to the main
             * poller interval. (Default is 60s.)
             */
            public Builder withComputeFrequencyMillis(long frequencyMillis)
            {
                this.frequencyMillis = frequencyMillis;
                return this;
            }

            /**
             * Create a new StatsConfig object.
             */
            public StatsConfig build()
            {
                return new StatsConfig(this);
            }
        }

        private bool publishCount;
        private bool publishTotal;
        private bool publishMin;
        private bool publishMax;
        private bool publishMean;
        private bool publishVariance;
        private bool publishStdDev;
        private double[] percentiles;
        private int sampleSize;
        private long frequencyMillis;

        /**
         * Creates a new configuration object for stats gathering.
         */
        public StatsConfig(Builder builder)
        {
            this.publishCount = builder.publishCount;
            this.publishTotal = builder.publishTotal;
            this.publishMin = builder.publishMin;
            this.publishMax = builder.publishMax;

            this.publishMean = builder.publishMean;
            this.publishVariance = builder.publishVariance;
            this.publishStdDev = builder.publishStdDev;
            this.sampleSize = builder.sampleSize;
            this.frequencyMillis = builder.frequencyMillis;

            Array.Copy(builder.percentiles, this.percentiles, builder.percentiles.Length);
        }

        /**
         * Whether we should publish a 'count' statistic.
         */
        public bool getPublishCount()
        {
            return publishCount;
        }

        /**
         * Whether we should publish a 'totalTime' statistic.
         */
        public bool getPublishTotal()
        {
            return publishTotal;
        }

        /**
         * Whether we should publish a 'min' statistic.
         */
        public bool getPublishMin()
        {
            return publishMin;
        }

        /**
         * Whether we should publish a 'max' statistic.
         */
        public bool getPublishMax()
        {
            return publishMax;
        }

        /**
         * Whether we should publish an 'avg' statistic.
         */
        public bool getPublishMean()
        {
            return publishMean;
        }

        /**
         * Whether we should publish a 'variance' statistic.
         */
        public bool getPublishVariance()
        {
            return publishVariance;
        }

        /**
         * Whether we should publish a 'stdDev' statistic.
         */
        public bool getPublishStdDev()
        {
            return publishStdDev;
        }

        /**
         * Get the size of the buffer that we should use.
         */
        public int getSampleSize()
        {
            return sampleSize;
        }

        /**
         * Get the frequency at which we should update all stats.
         */
        public long getFrequencyMillis()
        {
            return frequencyMillis;
        }

        /**
         * Get a copy of the array that holds which percentiles we should compute. The percentiles
         * are in the interval (0.0, 100.0)
         */
        public double[] getPercentiles()
        {
            var temp = new List<double>(percentiles);

            return temp.ToArray();
        }

        public override string ToString()
        {
            return "StatsConfig{"
                + "publishCount=" + publishCount
                + ", publishTotal=" + publishTotal
                + ", publishMin=" + publishMin
                + ", publishMax=" + publishMax
                + ", publishMean=" + publishMean
                + ", publishVariance=" + publishVariance
                + ", publishStdDev=" + publishStdDev
                + ", percentiles=" + percentiles.ToString()
                + ", sampleSize=" + sampleSize
                + ", frequencyMillis=" + frequencyMillis
                + '}';
        }

        public override bool Equals(Object o)
        {
            if (this == o)
            {
                return true;
            }
            if (!(o is StatsConfig))
            {
                return false;
            }

            StatsConfig that = (StatsConfig)o;
            return frequencyMillis == that.frequencyMillis
                && publishCount == that.publishCount
                && publishMax == that.publishMax
                && publishMean == that.publishMean
                && publishMin == that.publishMin
                && publishStdDev == that.publishStdDev
                && publishTotal == that.publishTotal
                && publishVariance == that.publishVariance
                && sampleSize == that.sampleSize
                && percentiles.Equals(that.percentiles);

        }

        public override int GetHashCode()
        {
            int result = (publishCount ? 1 : 0);
            result = 31 * result + (publishTotal ? 1 : 0);
            result = 31 * result + (publishMin ? 1 : 0);
            result = 31 * result + (publishMax ? 1 : 0);
            result = 31 * result + (publishMean ? 1 : 0);
            result = 31 * result + (publishVariance ? 1 : 0);
            result = 31 * result + (publishStdDev ? 1 : 0);
            result = 31 * result + percentiles.GetHashCode();
            result = 31 * result + sampleSize;
            result = 31 * result + (int)(frequencyMillis ^ ((long)(((ulong)frequencyMillis) >> 32)));
            return result;
        }
    }
}
