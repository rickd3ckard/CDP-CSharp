/* 
 * Public domain software, no restrictions. 
 * Released by rickd3ckard: https://github.com/rickd3ckard 
 * See: https://unlicense.org/
 */

using System.ComponentModel;
using System.Text.Json;

namespace CDP.Commands
{
    public class DOMScrollIntoViewIfNeeded
    {
        public DOMScrollIntoViewIfNeeded(int Id, int NodeId)
        {
            this.Id = Id;
            this.Method = "DOM.scrollIntoViewIfNeeded";
            this.Params = new Dictionary<string, int>();
            this.Params.Add("nodeId", NodeId);
        }

        public int Id { get; }
        public string Method { get; }
        public Dictionary<string, int> Params { get; }

        public override string ToString()
        {
            JsonSerializerOptions options = new JsonSerializerOptions();
            options.WriteIndented = true;
            options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;

            return JsonSerializer.Serialize(this, options);
        }
    }
}