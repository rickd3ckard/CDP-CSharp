/* 
 * Public domain software, no restrictions. 
 * Released by rickd3ckard: https://github.com/rickd3ckard 
 * See: https://unlicense.org/
 */

using System.Text;
using System.Text.Json;

namespace CDP.Commands
{
    public class DOMDescribeNodeCommand
    {
        public DOMDescribeNodeCommand(int Id, int NodeId,  int Depth = -1 , bool Pierce = false)
        {
            this.Id = Id;
            this.Method = "DOM.describeNode";
            this.Params = new Dictionary<string, object>();
            Params.Add("nodeId", NodeId);
            Params.Add("depth", Depth);
            Params.Add("pierce", Pierce);
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

        public ArraySegment<byte> Encode()
        {
            string closeCommand = this.ToString();
            byte[] encodedCommand = Encoding.UTF8.GetBytes(closeCommand);
            ArraySegment<byte> buffer = new ArraySegment<byte>(encodedCommand);
            return buffer;
        }
    }
}
