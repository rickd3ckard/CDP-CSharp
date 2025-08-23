/* 
 * Public domain software, no restrictions. 
 * Released by rickd3ckard: https://github.com/rickd3ckard 
 * See: https://unlicense.org/
 */

using System.Text.Json;
using WebSocketSharp;

namespace CDP.Objects
{
    public class Node
    {
        public Node(int NodeId, WebSocket webSocket)
        {
            this.NodeId = NodeId;
            this.WebSocket = webSocket;

            EventHandler<CloseEventArgs> closeEvent = (sender, e) => {
                throw new InvalidOperationException();
            };

            this.WebSocket.OnClose += closeEvent;
        }

        public int NodeId { get; }
        private WebSocket WebSocket { get; }

        public override string ToString()
        {
            JsonSerializerOptions options = new JsonSerializerOptions();
            options.WriteIndented = true;
            options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;

            return JsonSerializer.Serialize(this, options);
        }
    }
}
