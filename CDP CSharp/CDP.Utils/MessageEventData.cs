/* 
 * Public domain software, no restrictions. 
 * Released by rickd3ckard: https://github.com/rickd3ckard 
 * See: https://unlicense.org/
 */

using System.Text.Json.Serialization;
using System.Text.Json;

namespace CDP.Utils
{
    public class MessageEventData
    {
        public MessageEventData()
        {
            this.Method = string.Empty;
            this.Params = new Dictionary<string, JsonDocument>();
        }

        [JsonPropertyName("method")]
        public string Method { get; set; }

        [JsonPropertyName("params")]
        public Dictionary<string, JsonDocument> Params { get; set; }

        public override string ToString()
        {
            JsonSerializerOptions options = new JsonSerializerOptions();
            options.WriteIndented = true;

            return JsonSerializer.Serialize(this, options);
        }
    }
}