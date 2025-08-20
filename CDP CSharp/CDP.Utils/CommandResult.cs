/* 
 * Public domain software, no restrictions. 
 * Released by rickd3ckard: https://github.com/rickd3ckard 
 * See: https://unlicense.org/
 */

using System.Text.Json;
using System.Text.Json.Serialization;

namespace CDP.Utils
{
    public class CommandResult
    {
        public CommandResult()
        {
            this.Id = -1;
            this.Result = JsonDocument.Parse("{}");
        }

        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("result")]
        public JsonDocument Result { get; set; }

        public override string ToString()
        {
            JsonSerializerOptions options = new JsonSerializerOptions();
            options.WriteIndented = true;

            return JsonSerializer.Serialize(this, options);
        }
    }
}
