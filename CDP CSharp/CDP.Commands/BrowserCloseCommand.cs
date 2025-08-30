/* 
 * Public domain software, no restrictions. 
 * Released by rickd3ckard: https://github.com/rickd3ckard 
 * See: https://unlicense.org/
 */

using System.Text;
using System.Text.Json;

namespace CDP.Commands
{
    public class BrowserCloseCommand
    {
        public BrowserCloseCommand(int Id)
        {
            this.Id = Id;
            this.Method = "Browser.close";
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

        public ArraySegment<byte> Encode()
        {
            string closeCommand = this.ToString();
            byte[] encodedCommand = Encoding.UTF8.GetBytes(closeCommand);
            ArraySegment<byte> buffer = new ArraySegment<byte>(encodedCommand);
            return buffer;
        }
    }
}
