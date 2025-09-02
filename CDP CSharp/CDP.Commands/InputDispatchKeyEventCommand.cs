/* 
 * Public domain software, no restrictions. 
 * Released by rickd3ckard: https://github.com/rickd3ckard 
 * See: https://unlicense.org/
 */

using CDP.Utils;
using System.Text;
using System.Text.Json;

namespace CDP.Commands
{
    public class InputDispatchKeyEventCommand
    {
        public InputDispatchKeyEventCommand(int Id, KeyEventTypeEnum KeyType, Char? Text = null, string? Key = null, string? Code = null,
            int? WindowsVirtualKeyCode = null, int? NativeVirtualKeyCode = null, int? Modifiers = null) // typeEnum 
            
        {
            this.Id = Id;
            this.Method = "Input.dispatchKeyEvent";
            this.Params = new Dictionary<string, object>();
            this.Params.Add("type", KeyType.ToString());
            if (Text != null) { this.Params.Add("text", Text); }
            if (Key != null) { this.Params.Add("key", Key); }
            if (Code != null) { this.Params.Add("code", Code); }
            if (WindowsVirtualKeyCode != null) { this.Params.Add("windowsVirtualKeyCode", WindowsVirtualKeyCode); }
            if (NativeVirtualKeyCode != null) { this.Params.Add("nativeVirtualKeyCode", NativeVirtualKeyCode); }
            if (Modifiers != null) { this.Params.Add("modifiers", Modifiers); }
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

        public ArraySegment<byte> Encode()
        {
            string closeCommand = this.ToString();
            byte[] encodedCommand = Encoding.UTF8.GetBytes(closeCommand);
            ArraySegment<byte> buffer = new ArraySegment<byte>(encodedCommand);
            return buffer;
        }
    }
}
