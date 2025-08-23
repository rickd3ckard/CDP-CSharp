/* 
 * Public domain software, no restrictions. 
 * Released by rickd3ckard: https://github.com/rickd3ckard 
 * See: https://unlicense.org/
 */

using System.Text.Json;

namespace CDP.Commands
{
    public class DOMQuerySelectorCommad
    {
        public DOMQuerySelectorCommad(int Id, int NodeId, string Selector)
        {
            this.Id = Id;
            this.Method = "DOM.querySelector";
            this.Params = new Dictionary<string, object>();
            Params.Add("nodeId", NodeId);
            Params.Add("selector", Selector);
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
