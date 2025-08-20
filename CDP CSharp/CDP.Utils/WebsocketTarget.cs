/* 
 * Public domain software, no restrictions. 
 * Released by rickd3ckard: https://github.com/rickd3ckard 
 * See: https://unlicense.org/
 */

using System.Text.Json.Serialization;
using System.Text.Json;

namespace CDP.Utils
{
    public class WebsocketTarget
    {
        public WebsocketTarget()
        {
            this.Description = string.Empty;
            this.DevtoolsFrontendUrl = string.Empty;
            this.Id = string.Empty;
            this.Title = string.Empty;
            this.Type = string.Empty;
            this.URL = string.Empty;
            this.WebSocketDebuggerURL = string.Empty;
        }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("devtoolsFrontendUrl")]
        public string DevtoolsFrontendUrl { get; set; }

        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("url")]
        public string URL { get; set; }

        [JsonPropertyName("webSocketDebuggerUrl")]
        public string WebSocketDebuggerURL { get; set; }

        public override string ToString()
        {
            JsonSerializerOptions options = new JsonSerializerOptions();
            options.WriteIndented = true;

            return JsonSerializer.Serialize(this, options);
        }
    }
}