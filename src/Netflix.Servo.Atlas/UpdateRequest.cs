using System.Collections.Generic;
using Netflix.Servo.Tag;
using Netflix.Servo.Util;
using Newtonsoft.Json;

namespace Netflix.Servo.Atlas
{
    /**
  * A Request sent to the atlas-publish API.
  */
    public class UpdateRequest : JsonPayload
    {

        private TagList tags;
        private List<AtlasMetric> metrics;

        /**
         * Create an UpdateRequest to be sent to atlas.
         *
         * @param tags          common tags for all metrics in the request.
         * @param metricsToSend Array of metrics to send.
         * @param numMetrics    How many metrics in the array metricsToSend should be sent. Note
         *                      that this value needs to be lower or equal to metricsToSend.length
         */
        public UpdateRequest(TagList tags, Metric[] metricsToSend, int numMetrics)
        {
            Preconditions.checkArgument(metricsToSend.Length > 0, "metricsToSend is empty");
            Preconditions.checkArgument(numMetrics > 0 && numMetrics <= metricsToSend.Length,
                "numMetrics is 0 or out of bounds");

            this.metrics = new List<AtlasMetric>(numMetrics);
            for (int i = 0; i < numMetrics; ++i)
            {
                Metric m = metricsToSend[i];
                if (m.hasNumberValue())
                {
                    metrics.Add(new AtlasMetric(m));
                }
            }

            this.tags = tags;
        }

        TagList getTags()
        {
            return tags;
        }

        List<AtlasMetric> getMetrics()
        {
            return metrics;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is UpdateRequest))
            {
                return false;
            }
            UpdateRequest req = (UpdateRequest)obj;
            return tags.Equals(req.getTags())
                && metrics.Equals(req.getMetrics());
        }

        public override int GetHashCode()
        {
            int startValue = 23;
            int multiplier = 77;

            int hashCode = startValue;
            hashCode = hashCode * multiplier ^ tags.GetHashCode();
            hashCode = hashCode * multiplier ^ metrics.GetHashCode();

            return hashCode;
        }

        public override string ToString()
        {
            return "UpdateRequest{tags=" + tags + ", metrics=" + metrics + '}';
        }

        public void toJson(JsonTextWriter gen)
        {
            gen.WriteStartObject();

            // common tags
            gen.WritePropertyName("tags");
            gen.WriteStartArray();
            foreach (ITag tag in tags)
            {
                gen.WritePropertyName(ValidCharacters.toValidCharset(tag.getKey()));
                gen.WriteValue(ValidCharacters.toValidCharset(tag.getValue()));
            }
            gen.WriteEndArray();

            gen.WritePropertyName("metrics");
            gen.WriteStartArray();
            foreach (AtlasMetric m in metrics)
            {
                m.toJson(gen);
            }
            gen.WriteEndArray();

            gen.WriteEndObject();
            gen.Flush();
        }

        //Original
        //public void toJson(JsonGenerator gen) throws IOException
        //{
        //    gen.writeStartObject();

        //    // common tags
        //    gen.writeObjectFieldStart("tags");
        //    for (Tag tag : tags) {
        //        gen.writeStringField(
        //            ValidCharacters.toValidCharset(tag.getKey()),
        //            ValidCharacters.toValidCharset(tag.getValue()));
        //    }
        //    gen.writeEndObject();

        //    gen.writeArrayFieldStart("metrics");
        //    for (AtlasMetric m : metrics) {
        //        m.toJson(gen);
        //    }
        //    gen.writeEndArray();

        //    gen.writeEndObject();
        //    gen.flush();
        //}
    }
}
