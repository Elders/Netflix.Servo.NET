using System;
using System.Collections.Generic;
using Java.Util.Concurrent.Atomic;
using Netflix.Servo.Util;

namespace Netflix.Servo.Stats
{
    /**
 * A simple circular buffer that records values, and computes useful stats.
 * This implementation is not thread safe.
 */
    public class StatsBuffer
    {
        private int count;
        private double mean;
        private double sumSquares;
        private double variance;
        private double stddev;
        private long min;
        private long max;
        private long total;

        private double[] percentiles;
        private double[] percentileValues;
        private int size;
        private long[] values;
        private AtomicBoolean statsComputed = new AtomicBoolean(false);

        /**
         * Create a circular buffer that will be used to record values and compute useful stats.
         *
         * @param size        The capacity of the buffer
         * @param percentiles Array of percentiles to compute. For example { 95.0, 99.0 }.
         *                    If no percentileValues are required pass a 0-sized array.
         */
        public StatsBuffer(int size, double[] percentiles)
        {
            Preconditions.checkArgument(size > 0, "Size of the buffer must be greater than 0");
            Preconditions.checkArgument(percentiles != null,
                "Percents array must be non-null. Pass a 0-sized array "
                    + "if you don't want any percentileValues to be computed.");
            Preconditions.checkArgument(validPercentiles(percentiles),
                "All percentiles should be in the interval (0.0, 100.0]");
            values = new long[size];
            this.size = size;
            Array.Copy(percentiles, this.percentiles, percentiles.Length);
            this.percentileValues = new double[percentiles.Length];

            reset();
        }

        private static bool validPercentiles(double[] percentiles)
        {
            foreach (var percentile in percentiles)
            {
                if (percentile <= 0.0 || percentile > 100.0)
                {
                    return false;
                }
            }
            return true;
        }

        /**
         * Reset our local state: All values are set to 0.
         */
        public void reset()
        {
            statsComputed.GetAndSet(false);
            count = 0;
            total = 0L;
            mean = 0.0;
            variance = 0.0;
            stddev = 0.0;
            min = 0L;
            max = 0L;
            sumSquares = 0.0;
            for (int i = 0; i < percentileValues.Length; ++i)
            {
                percentileValues[i] = 0.0;
            }
        }

        /**
         * Record a new value for this buffer.
         */
        public void record(long n)
        {
            values[count++ % size] = n;
            total += n;
            sumSquares += n * n;
        }

        /**
         * Compute stats for the current set of values.
         */
        public void computeStats()
        {
            if (statsComputed.GetAndSet(true))
            {
                return;
            }

            if (count == 0)
            {
                return;
            }

            int curSize = Math.Min(count, size);
            Array.Sort(values, 0, curSize); // to compute percentileValues
            min = values[0];
            max = values[curSize - 1];
            mean = (double)total / count;
            variance = (sumSquares / curSize) - (mean * mean);
            stddev = Math.Sqrt(variance);
            computePercentiles(curSize);
        }

        private void computePercentiles(int curSize)
        {
            for (int i = 0; i < percentiles.Length; ++i)
            {
                percentileValues[i] = calcPercentile(curSize, percentiles[i]);
            }
        }

        private double calcPercentile(int curSize, double percent)
        {
            if (curSize == 0)
            {
                return 0.0;
            }
            if (curSize == 1)
            {
                return values[0];
            }

            /*
             * We use the definition from http://cnx.org/content/m10805/latest
             * modified for 0-indexed arrays.
             */
            double rank = percent * curSize / 100.0; // SUPPRESS CHECKSTYLE MagicNumber
            int ir = (int)Math.Floor(rank);
            int irNext = ir + 1;
            double fr = rank - ir;
            if (irNext >= curSize)
            {
                return values[curSize - 1];
            }
            else if (fr == 0.0)
            {
                return values[ir];
            }
            else
            {
                // Interpolate between the two bounding values
                double lower = values[ir];
                double upper = values[irNext];
                return fr * (upper - lower) + lower;
            }
        }

        /**
         * Get the number of entries recorded.
         */
        public int getCount()
        {
            return count;
        }

        /**
         * Get the average of the values recorded.
         *
         * @return The average of the values recorded, or 0.0 if no values were recorded.
         */
        public double getMean()
        {
            return mean;
        }

        /**
         * Get the variance for the population of the recorded values present in our buffer.
         *
         * @return The variance.p of the values recorded, or 0.0 if no values were recorded.
         */
        public double getVariance()
        {
            return variance;
        }

        /**
         * Get the standard deviation for the population of the recorded values present in our buffer.
         *
         * @return The stddev.p of the values recorded, or 0.0 if no values were recorded.
         */
        public double getStdDev()
        {
            return stddev;
        }

        /**
         * Get the minimum of the values currently in our buffer.
         *
         * @return The min of the values recorded, or 0.0 if no values were recorded.
         */
        public long getMin()
        {
            return min;
        }

        /**
         * Get the max of the values currently in our buffer.
         *
         * @return The max of the values recorded, or 0.0 if no values were recorded.
         */
        public long getMax()
        {
            return max;
        }

        /**
         * Get the total sum of the values recorded.
         *
         * @return The sum of the values recorded, or 0.0 if no values were recorded.
         */
        public long getTotalTime()
        {
            return total;
        }

        /**
         * Get the computed percentileValues. See {@link StatsConfig} for how to request different
         * percentileValues. Note that for efficiency reasons we return the actual array of
         * computed values.
         * Users must NOT modify this array.
         *
         * @return An array of computed percentileValues.
         */
        public double[] getPercentileValues()
        {
            var temp = new List<double>(percentileValues);

            return temp.ToArray();
        }

        /**
         * Return the percentiles we will compute: For example: 95.0, 99.0.
         */
        public double[] getPercentiles()
        {
            var temp = new List<double>(percentiles);

            return temp.ToArray();
        }

        /**
         * Return the value for the percentile given an index.
         * @param index If percentiles are [ 95.0, 99.0 ] index must be 0 or 1 to get the 95th
         *              or 99th percentile respectively.
         *
         * @return The value for the percentile requested.
         */
        public double getPercentileValueForIdx(int index)
        {
            return percentileValues[index];
        }
    }
}
