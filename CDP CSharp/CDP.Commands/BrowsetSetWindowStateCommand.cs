/* 
 * Public domain software, no restrictions. 
 * Released by rickd3ckard: https://github.com/rickd3ckard 
 * See: https://unlicense.org/
 */

using CDP.Utils;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace CDP.Commands
{
    public class BrowsetSetWindowStateCommand
    {
        public BrowsetSetWindowStateCommand(int Id, int WindowsId, WindowStateEnum WindowState)
        {
            this.Id = Id;
            this.Method = "Browser.setWindowBounds";
            this.Params = new Dictionary<string, object>();
            Params.Add("windowId", WindowsId);
            Params.Add("bounds", new Bounds(WindowState: WindowState));
        }

        public int Id { get; }
        public string Method { get; }
        public Dictionary<string, object> Params { get; }

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
