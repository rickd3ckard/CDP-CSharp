/* 
 * Public domain software, no restrictions. 
 * Released by rickd3ckard: https://github.com/rickd3ckard 
 * See: https://unlicense.org/
 */

using System.Text.Json;

namespace CDP.Commands
{
    public class DOMGetDocumentCommand
    {
        public DOMGetDocumentCommand(int Id, int Depth = 1, bool Pierce = false)
        {
            this.Id = Id;
            this.Method = "DOM.getDocument";
            this.Params = new Dictionary<string, object>();
            this.Params.Add("depth", Depth);
            this.Params.Add("pierce", Pierce);
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
