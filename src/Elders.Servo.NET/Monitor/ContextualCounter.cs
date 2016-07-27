//using System;
//using Elders.Servo.NET.Tag;

//namespace Elders.Servo.NET.Monitor
//{
//    /**
//  * Composite that maintains separate simple counters for each distinct set of tags returned by the
//  * tagging context.
//  */
//    public class ContextualCounter : AbstractContextualMonitor<object, Counter>, Counter
//    {
//        /**
//         * Create a new instance of the counter.
//         *
//         * @param config     shared configuration
//         * @param context    provider for context specific tags
//         * @param newMonitor function to create new counters
//         */
//        public ContextualCounter(MonitorConfig config, TaggingContext context, Func<MonitorConfig, Counter> newMonitor)
//            : base(config, context, newMonitor)
//        {

//        }

//        public void increment()
//        {
//            getMonitorForCurrentContext().increment();
//        }

//        public void increment(long amount)
//        {
//            getMonitorForCurrentContext().increment(amount);
//        }

//        public override object getValue(int pollerIndex)
//        {
//            return getMonitorForCurrentContext().getValue(pollerIndex);
//        }
//    }
//}
