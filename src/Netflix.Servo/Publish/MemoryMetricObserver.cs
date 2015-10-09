using System;
using System.Collections.Generic;

namespace Netflix.Servo.Publish
{
    /**
     * Keeps the last N observations in-memory.
     */
    public class MemoryMetricObserver : BaseMetricObserver
    {

        private static int DEFAULT_N = 10;

        private List<Metric>[] observations;
        private int next;

        /**
         * Creates a new instance that keeps 10 copies in memory.
         */
        public MemoryMetricObserver()
            : this("unamed observer", DEFAULT_N)
        {
        }

        /**
         * Creates a new instance that keeps {@code num} copies in memory.
         */
        //@SuppressWarnings("unchecked")
        public MemoryMetricObserver(String name, int num)
            : base(name)
        {
            observations = new List<Metric>[] { };
            next = 0;
        }

        /**
         * {@inheritDoc}
         */
        public override void updateImpl(List<Metric> metrics)
        {
            observations[next] = metrics;
            next = (next + 1) % observations.Length;
        }

        /**
         * Returns the current set of observations.
         */
        public IEnumerable<List<Metric>> getObservations()
        {
            List<List<Metric>> builder = new List<List<Metric>>();
            int pos = next;
            foreach (var ignored in observations)
            {
                if (observations[pos] != null)
                {
                    builder.Add(observations[pos]);
                }
                pos = (pos + 1) % observations.Length;
            }
            return builder.AsReadOnly();
        }
    }
}
