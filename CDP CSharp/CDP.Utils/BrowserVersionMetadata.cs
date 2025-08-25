/* 
 * Public domain software, no restrictions. 
 * Released by rickd3ckard: https://github.com/rickd3ckard 
 * See: https://unlicense.org/
 */

using System.Text.Json.Serialization;
using System.Text.Json;

namespace CDP.Utils
{
    public class BrowserVersionMetadata
    {
        public BrowserVersionMetadata() { }

        [JsonPropertyName("Browser")]
        public string? Browser { get; set; }

        [JsonPropertyName("Protocol-Version")]
        public string? ProtocolVersion { get; set; }

        [JsonPropertyName("User-Agent")]
        public string? UserAgent { get; set; }

        [JsonPropertyName("V8-Version")]
        public string?V8Version { get; set; }

        [JsonPropertyName("WebKit-Version")]
        public string? WebkitVersion { get; set; }

        [JsonPropertyName("webSocketDebuggerUrl")]
        public string? WebSocketDebuggerUrl { get; set; }

        public override string ToString()
        {
            JsonSerializerOptions options = new JsonSerializerOptions();
            options.WriteIndented = true;
            options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;

            return JsonSerializer.Serialize(this, options);
        }
    }
}
