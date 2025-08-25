/* 
 * Public domain software, no restrictions. 
 * Released by rickd3ckard: https://github.com/rickd3ckard 
 * See: https://unlicense.org/
 */

using System.Text.Json;

namespace CDP.Commands
{
    public class PageGetLayoutMetricsCommand
    {
        public PageGetLayoutMetricsCommand(int Id)
        {
            this.Id = Id;
            this.Method = "Page.getLayoutMetrics";
        }

        public int Id { get; }
        public string Method { get; }

        public override string ToString()
        {
            JsonSerializerOptions options = new JsonSerializerOptions();
            options.WriteIndented = true;
            options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;

            return JsonSerializer.Serialize(this, options);
        }
    }
}
