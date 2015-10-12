//using System;
//using System.Collections.Generic;
//using Netflix.Servo.Tag;
//using Netflix.Servo.Util;

//namespace Netflix.Servo.Monitor
//{
//    /**
// * A simple timer implementation providing the total time, count, min, and max for the times that
// * have been recorded.
// */
//    public class BasicTimer : AbstractMonitor<long>, Timer, CompositeMonitor<long>
//    {

//        private static String STATISTIC = "statistic";
//        private static String UNIT = "unit";

//        private static ITag STAT_TOTAL = Tags.newTag(STATISTIC, "totalTime");
//        private static ITag STAT_COUNT = Tags.newTag(STATISTIC, "count");
//        private static ITag STAT_TOTAL_SQ = Tags.newTag(STATISTIC, "totalOfSquares");
//        private static ITag STAT_MAX = Tags.newTag(STATISTIC, "max");

//        private double timeUnitNanosFactor;
//        private StepCounter totalTime;
//        private StepCounter count;
//        private DoubleCounter totalOfSquares;
//        private MaxGauge max;

//        private List<Monitor<long>> monitors;

//        internal class FactorMonitor<T> : AbstractMonitor<Double>, NumericMonitor<Double>
//        {
//            private Monitor<T> wrapped;
//            private double factor;

//            internal FactorMonitor(Monitor<T> wrapped, double factor)
//                : base(wrapped.getConfig())
//            {
//                this.wrapped = wrapped;
//                this.factor = factor;
//            }

//            public override Double getValue(int pollerIndex)
//            {
//                return wrapped.getValue(pollerIndex) * factor;
//            }
//        }

//        /**
//         * Creates a new instance of the timer with a unit of milliseconds.
//         */
//        public BasicTimer(MonitorConfig config)
//            : this(config, ClockWithOffset.INSTANCE)
//        {

//        }

//        /**
//         * Creates a new instance of the timer.
//         */
//        BasicTimer(MonitorConfig config, Clock clock)
//            : base(config)
//        {


//            ITag unitTag = Tags.newTag(UNIT, "MILLISECONDS");
//            MonitorConfig unitConfig = config.withAdditionalTag(unitTag);
//            timeUnitNanosFactor = 1.0 / TimeSpan.FromMilliseconds(1).TotalNanoseconds();

//            totalTime = new StepCounter(unitConfig.withAdditionalTag(STAT_TOTAL), clock);
//            count = new StepCounter(unitConfig.withAdditionalTag(STAT_COUNT), clock);
//            totalOfSquares = new DoubleCounter(unitConfig.withAdditionalTag(STAT_TOTAL_SQ), clock);
//            max = new MaxGauge(unitConfig.withAdditionalTag(STAT_MAX), clock);

//            FactorMonitor<long> totalTimeFactor = new FactorMonitor<long>(totalTime, timeUnitNanosFactor);
//            FactorMonitor<double> totalSquaresFactor = new FactorMonitor<double>(totalOfSquares, timeUnitNanosFactor * timeUnitNanosFactor);
//            FactorMonitor<long> maxFactor = new FactorMonitor<long>(max, timeUnitNanosFactor);

//            monitors = UnmodifiableList.< Monitor <?>> of(totalTimeFactor, count,
//                totalSquaresFactor, maxFactor);
//        }

//        public List<Monitor<long>> getMonitors()
//        {
//            return monitors;
//        }

//        public Stopwatch start()
//        {
//            Stopwatch s = new TimedStopwatch(this);
//            s.start();
//            return s;
//        }

//        public TimeSpan getTimeUnit()
//        {
//            return timeUnit;
//        }

//        private void recordNanos(long nanos)
//        {
//            if (nanos >= 0)
//            {
//                totalTime.increment(nanos);
//                count.increment();
//                totalOfSquares.increment((double)nanos * (double)nanos);
//                max.update(nanos);
//            }
//        }

//        [Obsolete]
//        public void record(long duration)
//        {
//            long nanos = timeUnit.toNanos(duration);
//            recordNanos(nanos);
//        }

//        public void record(TimeSpan duration)
//        {
//            recordNanos(duration.TotalNanoseconds());
//        }

//        private double getTotal(int pollerIndex)
//        {
//            return totalTime.getCurrentCount(pollerIndex) * timeUnitNanosFactor;
//        }


//        public override long getValue(int pollerIndex)
//        {
//            long cnt = count.getCurrentCount(pollerIndex);
//            long value = (long)(getTotal(pollerIndex) / cnt);
//            return (cnt == 0) ? 0L : value;
//        }

//        /**
//         * Get the total time for all updates.
//         */
//        public Double getTotalTime()
//        {
//            return getTotal(0);
//        }

//        /**
//         * Get the total number of updates.
//         */
//        public long getCount()
//        {
//            return count.getCurrentCount(0);
//        }

//        /**
//         * Get the max value since the last reset.
//         */
//        public Double getMax()
//        {
//            return max.getCurrentValue(0) * timeUnitNanosFactor;
//        }

//        public override bool Equals(Object obj)
//        {
//            if (this == obj)
//            {
//                return true;
//            }
//            if (obj == null || !(obj is BasicTimer))
//            {
//                return false;
//            }
//            BasicTimer m = (BasicTimer)obj;
//            return config.Equals(m.getConfig())
//                && totalTime.Equals(m.totalTime)
//                && count.Equals(m.count)
//                && totalOfSquares.Equals(m.totalOfSquares)
//                && max.Equals(m.max);
//        }


//        public override int GetHashCode()
//        {
//            int result = config.GetHashCode();
//            result = 31 * result + totalTime.GetHashCode();
//            result = 31 * result + count.GetHashCode();
//            result = 31 * result + totalOfSquares.GetHashCode();
//            result = 31 * result + max.GetHashCode();
//            return result;
//        }

//        public override String ToString()
//        {
//            return "BasicTimer{config=" + config + ", totalTime=" + totalTime
//                + ", count=" + count + ", totalOfSquares=" + totalOfSquares
//                + ", max=" + max + '}';
//        }
//    }
//}
