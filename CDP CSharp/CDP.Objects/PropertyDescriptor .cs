/* 
 * Public domain software, no restrictions. 
 * Released by rickd3ckard: https://github.com/rickd3ckard 
 * See: https://unlicense.org/
 */

using System.Text.Json;
using System.Text.Json.Serialization;

namespace CDP.Objects
{
    public class PropertyDescriptor
    {
        public PropertyDescriptor() { }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("value")]
        public RemoteObject? Value { get; set; }

        [JsonPropertyName("writable")]
        public bool? Writable { get; set; }

        [JsonPropertyName("get")]
        public RemoteObject? Get { get; set; }

        [JsonPropertyName("set")]
        public RemoteObject? Set { get; set; }

        [JsonPropertyName("configurable")]
        public bool? Configurable { get; set; }

        [JsonPropertyName("enumerable")]
        public bool? Enumerable { get; set; }

        [JsonPropertyName("wasThrown")]
        public bool? WasThrown { get; set; }

        [JsonPropertyName("isOwn")]
        public bool? IsOwn { get; set; }

        [JsonPropertyName("symbol")]
        public RemoteObject? Symbol { get; set; }

        public override string ToString()
        {
            JsonSerializerOptions options = new JsonSerializerOptions();
            options.WriteIndented = true;
            options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            options.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;

            return JsonSerializer.Serialize(this, options);
        }
    }
}