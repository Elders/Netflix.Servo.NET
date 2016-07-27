using System;
using System.Linq;
using Java.Util.Concurrent.Atomic;
using Elders.Servo.NET.Util;

namespace Elders.Servo.NET.Monitor
{
    /**
 * Utility class for managing a set of AtomicLong instances mapped to a particular step interval.
 * The current implementation keeps an array of with two items where one is the current value
 * being updated and the other is the value from the previous interval and is only available for
 * polling.
 */
    internal class StepLong
    {
        private const int PREVIOUS = 0;
        private const int CURRENT = 1;

        private long init;
        private Clock clock;

        private AtomicLong[] data;

        private AtomicLong[] lastPollTime;

        private AtomicLong[] lastInitPos;

        internal StepLong(long init, Clock clock)
        {
            this.init = init;
            this.clock = clock;
            lastInitPos = new AtomicLong[Pollers.NUM_POLLERS];
            lastPollTime = new AtomicLong[Pollers.NUM_POLLERS];
            for (int i = 0; i < Pollers.NUM_POLLERS; ++i)
            {
                lastInitPos[i] = new AtomicLong(0L);
                lastPollTime[i] = new AtomicLong(0L);
            }
            data = new AtomicLong[2 * Pollers.NUM_POLLERS];
            for (int i = 0; i < data.Length; ++i)
            {
                data[i] = new AtomicLong(init);
            }
        }

        internal void addAndGet(long amount)
        {
            for (int i = 0; i < Pollers.NUM_POLLERS; ++i)
            {
                getCurrent(i).AddAndGet(amount);
            }
        }

        private void rollCount(int pollerIndex, long now)
        {
            long step = Pollers.POLLING_INTERVALS[pollerIndex];
            long stepTime = now / step;
            long lastInit = lastInitPos[pollerIndex].Value;
            if (lastInit < stepTime && lastInitPos[pollerIndex].CompareAndSet(lastInit, stepTime))
            {
                int prev = 2 * pollerIndex + PREVIOUS;
                int curr = 2 * pollerIndex + CURRENT;
                data[prev].GetAndSet(data[curr].GetAndSet(init));
            }
        }

        internal AtomicLong getCurrent(int pollerIndex)
        {
            rollCount(pollerIndex, clock.now());
            return data[2 * pollerIndex + CURRENT];
        }

        internal Datapoint poll(int pollerIndex)
        {
            long now = clock.now();
            long step = Pollers.POLLING_INTERVALS[pollerIndex];

            rollCount(pollerIndex, now);
            int prevPos = 2 * pollerIndex + PREVIOUS;
            long value = data[prevPos].Value;

            long last = lastPollTime[pollerIndex].GetAndSet(now);
            long missed = (now - last) / step - 1;

            long stepStart = now / step * step;
            if (last / step == now / step)
            {
                return new Datapoint(stepStart, value);
            }
            else if (last > 0L && missed > 0L)
            {
                return Datapoint.UNKNOWN;
            }
            else
            {
                return new Datapoint(stepStart, value);
            }
        }


        public override String ToString()
        {
            return "StepLong{init=" + init
                + ", data=" + string.Concat(data.Select(x => x.ToString()))
                + ", lastPollTime=" + string.Concat(lastPollTime.Select(x => x.ToString()))
                + ", lastInitPos=" + string.Concat(lastInitPos.Select(x => x.ToString())) + '}';
        }
    }
}