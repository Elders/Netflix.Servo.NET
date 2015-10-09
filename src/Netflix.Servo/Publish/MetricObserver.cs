using System;
using System.Collections.Generic;

namespace Netflix.Servo.Publish
{
    public interface MetricObserver
    {
        /**
         * Invoked with the most recent values for a set of metrics.
         */
        void update(List<Metric> metrics);

        /**
         * Name associated with an observer. Mostly used to make log messages more
         * informative.
         */
        String getName();
    }
}
