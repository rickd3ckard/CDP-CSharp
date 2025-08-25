/* 
 * Public domain software, no restrictions. 
 * Released by rickd3ckard: https://github.com/rickd3ckard 
 * See: https://unlicense.org/
 */

using System.Text.Json;

namespace CDP.Commands
{
    public class InputDispatchKeyEventCommand
    {
        public InputDispatchKeyEventCommand(int Id, string Type, Char Key) // typeEnum 
        {
            this.Id = Id;
            this.Method = "Input.dispatchKeyEvent";
            this.Params = new Dictionary<string, object>();
            this.Params.Add("type", Type);
            this.Params.Add("text", Key);
        }

        public int Id { get; }
        public string Method { get; }
        public Dictionary<string, object> Params { get; }

        public override string ToString()
        {
            JsonSerializerOptions options = new JsonSerializerOptions();
            options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            options.WriteIndented = true;

            return JsonSerializer.Serialize(this, options);
        }
    }
}
