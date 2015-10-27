//using System;
//using System.Collections.Generic;
//using Java.Util.Concurrent.Atomic;
//using Netflix.Servo.Attributes;
//using Netflix.Servo.Tag;
//using Netflix.Servo.Util;

//namespace Netflix.Servo.Publish
//{
//    public abstract class BaseMetricObserver : MetricObserver
//    {
//        [MonitorTags]
//        private ITagList tags;

//        private String name;

//        /**
//         * Total number of times update has been called.
//         */
//        [Monitor(name = "updateCount", type = DataSourceTypeEnum.COUNTER)]
//        private AtomicInteger updateCount = new AtomicInteger(0);

//        /**
//         * Number of times update failed with an exception.
//         */
//        [Monitor(name = "updateFailureCount", type = DataSourceTypeEnum.COUNTER)]
//        private AtomicInteger failedUpdateCount = new AtomicInteger(0);

//        /**
//         * Creates a new instance with a given name.
//         */
//        public BaseMetricObserver(String name)
//        {
//            ITag id = Tags.newTag(StandardTagKeys.MONITOR_ID.getKeyName(), name);
//            this.name = Preconditions.checkNotNull(name, "name");
//            this.tags = BasicTagList.of(id);
//        }

//        /**
//         * Update method that should be defined by sub-classes. This method will
//         * get invoked and counts will be maintained in the base observer.
//         */
//        public abstract void updateImpl(List<Metric> metrics);

//        /**
//         * {@inheritDoc}
//         */
//        public void update(List<Metric> metrics)
//        {
//            Preconditions.checkNotNull(metrics, "metrics");
//            try
//            {
//                updateImpl(metrics);
//            }
//            catch (Exception t)
//            {
//                failedUpdateCount.IncrementAndGet();
//                throw;
//            }
//            finally
//            {
//                updateCount.IncrementAndGet();
//            }
//        }

//        public String getName()
//        {
//            return name;
//        }

//        /**
//         * Can be used by sub-classes to increment the failed count if they handle
//         * exception internally.
//         */
//        protected void incrementFailedCount()
//        {
//            failedUpdateCount.IncrementAndGet();
//        }

//        /**
//         * Returns the total number of times update has been called.
//         */
//        public int getUpdateCount()
//        {
//            return updateCount.Value;
//        }

//        /**
//         * Returns the number of times update failed with an exception.
//         */
//        public int getFailedUpdateCount()
//        {
//            return failedUpdateCount.Value;
//        }
//    }
//}
