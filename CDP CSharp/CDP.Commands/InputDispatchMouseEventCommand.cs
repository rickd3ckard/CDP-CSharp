/* 
 * Public domain software, no restrictions. 
 * Released by rickd3ckard: https://github.com/rickd3ckard 
 * See: https://unlicense.org/
 */

using CDP.Utils;
using System.Text.Json;

namespace CDP.Commands
{
    public class InputDispatchMouseEventCommand
    {
        public InputDispatchMouseEventCommand(int Id, MouseButtonEnum MouseButton, MouseEventTypeEnum EventType, Point MousePosition)
        {
            this.Id = Id;
            this.Method = "Input.dispatchMouseEvent";
            this.Params = new Dictionary<string, object>();
            this.Params.Add("type", EventType.ToString());
            this.Params.Add("x", MousePosition.X);
            this.Params.Add("y", MousePosition.Y);
            this.Params.Add("button", MouseButton.ToString());
            this.Params.Add("clickCount", 1);
        }

        public int Id { get; }
        public string Method { get; }
        public Dictionary<string, object> Params { get; }

        public override string ToString()
        {
            JsonSerializerOptions options = new JsonSerializerOptions();
            options.WriteIndented = true;
            options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;

            return JsonSerializer.Serialize(this, options);
        }
    }
}
