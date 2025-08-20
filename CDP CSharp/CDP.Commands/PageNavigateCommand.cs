/* 
 * Public domain software, no restrictions. 
 * Released by rickd3ckard: https://github.com/rickd3ckard 
 * See: https://unlicense.org/
 */

using System.Text.Json;

namespace CDP.Commands
{
    public class PageNavigateCommand
    {
        public PageNavigateCommand(int Id, string URL)
        {
            this.Id = Id;
            this.Method = "Page.navigate";
            this.Params = new Dictionary<string, string>();
            this.Params.Add("url", URL);
        }

        public int Id { get; }
        public string Method { get; }
        public Dictionary<string, string> Params { get; }

        public override string ToString()
        {
            JsonSerializerOptions options = new JsonSerializerOptions();
            options.WriteIndented = true;
            options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;

            return JsonSerializer.Serialize(this, options);
        }
    }
}