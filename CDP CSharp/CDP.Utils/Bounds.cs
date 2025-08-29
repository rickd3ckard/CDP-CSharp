/* 
 * Public domain software, no restrictions. 
 * Released by rickd3ckard: https://github.com/rickd3ckard 
 * See: https://unlicense.org/
 */

using System.Text.Json;
using System.Text.Json.Serialization;

namespace CDP.Utils
{
    public class Bounds 
    {
        public Bounds(int? Left = null, int? Top = null, int? Width = null, int? Height = null, WindowStateEnum? WindowState = null) 
        {
            this.Left = Left;
            this.Top = Top;
            this.Width = Width;
            this.Height = Height;
            this.WindowState = WindowState.ToString();

            // if windowstate is set all other must be null => add invalid operation exception
        }

        public int? Left { get; set; }
        public int? Top { get; set; }
        public int? Width { get; set; }
        public int? Height { get; set; }
        public string? WindowState { get; set; }

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
