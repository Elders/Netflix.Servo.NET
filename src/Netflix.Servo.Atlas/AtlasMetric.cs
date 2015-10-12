using System;
using Netflix.Servo.Monitor;
using Netflix.Servo.Tag;
using Netflix.Servo.Util;
using Newtonsoft.Json;

namespace Netflix.Servo.Atlas
{
    /**
  * A metric that can be reported to Atlas.
  */
    public class AtlasMetric : JsonPayload
    {
        private MonitorConfig config;
        private long start;
        private double value;

        public AtlasMetric(Metric m)
            : this(m.getConfig(), m.getTimestamp(), m.getNumberValue())
        {

        }

        AtlasMetric(MonitorConfig config, long start, object value)
        {
            this.config = Preconditions.checkNotNull(config, "config");
            this.value = (double)Preconditions.checkNotNull(value, "value");
            this.start = start;
        }

        MonitorConfig getConfig()
        {
            return config;
        }

        long getStartTime()
        {
            return start;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is AtlasMetric))
            {
                return false;
            }
            AtlasMetric m = (AtlasMetric)obj;
            return config.Equals(m.getConfig())
                && start == m.getStartTime()
                && value.Equals(m.value);
        }


        public override int GetHashCode()
        {
            int startValue = 23;
            int multiplier = 77;

            int hashCode = startValue;
            hashCode = hashCode * multiplier ^ config.GetHashCode();
            hashCode = hashCode * multiplier ^ start.GetHashCode();
            hashCode = hashCode * multiplier ^ value.GetHashCode();

            return hashCode;
        }


        public override String ToString()
        {
            return "AtlasMetric{config=" + config
                + ", start=" + start + ", value=" + value + '}';
        }

        public void toJson(JsonTextWriter gen)
        {
            //StringBuilder sb = new StringBuilder();
            //StringWriter sw = new StringWriter(sb);
            //JsonWriter jsonWriter = new JsonTextWriter(sw);

            //jsonWriter.Formatting = Formatting.Indented;
            gen.WriteStartObject();

            gen.WritePropertyName("tags");
            gen.WriteStartArray();
            gen.WritePropertyName("name");
            gen.WriteValue(config.getName());

            foreach (ITag tag in config.getTags())
            {
                gen.WritePropertyName(tag.getKey());
                gen.WriteValue(tag.getValue());
            }
            gen.WriteEndArray();

            gen.WritePropertyName("start");
            gen.WriteValue(start);
            gen.WritePropertyName("value");
            gen.WriteValue(value);

            gen.WriteEndObject();
            gen.Flush();
        }

        //Original
        //public void toJson(JsonGenerator gen) throws IOException
        //{
        //    gen.writeStartObject();

        //    gen.writeObjectFieldStart("tags");
        //    gen.writeStringField("name", config.getName());
        //    for (Tag tag : config.getTags()) {
        //        gen.writeStringField(tag.getKey(), tag.getValue());
        //    }
        //    gen.writeEndObject();

        //    gen.writeNumberField("start", start);
        //    gen.writeNumberField("value", value);

        //    gen.writeEndObject();
        //    gen.flush();
        //}
    }
}
