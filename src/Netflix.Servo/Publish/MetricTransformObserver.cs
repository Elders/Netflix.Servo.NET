using System;
using System.Collections.Generic;
using System.Linq;

namespace Netflix.Servo.Publish
{
    /**
 * An observer that will transform the list of metrics using a given function.
 */
    public class MetricTransformObserver : MetricObserver
    {
        private Func<Metric, Metric> transformer;
        private MetricObserver observer;

        /**
         * Create a new MetricTransformObserver using the given transfomer function.
         *
         * @param transformer The function used to transform metrics.
         * @param observer    The MetricObserver that will receive the transfomed metrics.
         */
        public MetricTransformObserver(Func<Metric, Metric> transformer, MetricObserver observer)
        {
            this.transformer = transformer;
            this.observer = observer;
        }

        public void update(List<Metric> metrics)
        {
            List<Metric> transformed = metrics.Select(x => transformer(x)).ToList();
            observer.update(transformed);
        }

        public String getName()
        {
            return "MetricTransformObserver";
        }
    }
}
