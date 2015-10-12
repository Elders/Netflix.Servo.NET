using System;

namespace Netflix.Servo.Monitor
{
    /**
  * Tuple for a timestamp and value.
  */
    internal class Datapoint
    {

        public static Datapoint UNKNOWN = new Datapoint(0L, -1L);

        private long timestamp;
        private long value;

        internal Datapoint(long timestamp, long value)
        {
            this.timestamp = timestamp;
            this.value = value;
        }

        internal bool isUnknown()
        {
            return (timestamp == 0L);
        }

        internal long getTimestamp()
        {
            return timestamp;
        }

        internal long getValue()
        {
            return value;
        }


        public override int GetHashCode()
        {
            int result = (int)(timestamp ^ (long)((ulong)timestamp >> 32));
            result = 31 * result + (int)(value ^ (long)((ulong)value >> 32));
            return result;
        }

        public override bool Equals(Object obj)
        {
            if (obj == null || !(obj is Datapoint))
            {
                return false;
            }
            Datapoint dp = (Datapoint)obj;
            return timestamp == dp.timestamp && value == dp.value;
        }

        public override String ToString()
        {
            return "Datapoint{timestamp=" + timestamp + ", value=" + value + '}';
        }
    }

}
