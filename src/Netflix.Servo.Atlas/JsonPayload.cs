using Newtonsoft.Json;

namespace Netflix.Servo.Atlas
{
    /**
  * payload that can be serialized to json.
  */
    public interface JsonPayload
    {

        /**
         * Serialize the current entity to JSON using the given generator.
         */
        void toJson(JsonTextWriter gen);
    }
}
