/* 
 * Public domain software, no restrictions. 
 * Released by rickd3ckard: https://github.com/rickd3ckard 
 * See: https://unlicense.org/
 */

using System.Text.Json;
using System.Text.Json.Serialization;

namespace CDP.Utils
{
    public class LayoutViewport
    {
        public LayoutViewport() { }

        [JsonPropertyName("pageX")]
        public int PageX { get; set; }

        [JsonPropertyName("pageY")]
        public int PageY { get; set; }

        [JsonPropertyName("clientWidth")]
        public int ClientWidth { get; set; }

        [JsonPropertyName("clientHeight")]
        public int ClientHight { get; set; }

        public override string ToString()
        {
            JsonSerializerOptions options = new JsonSerializerOptions();
            options.WriteIndented = true;

            return JsonSerializer.Serialize(this, options);
        }
    }
}
