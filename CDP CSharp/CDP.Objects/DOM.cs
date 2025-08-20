/* 
 * Public domain software, no restrictions. 
 * Released by rickd3ckard: https://github.com/rickd3ckard 
 * See: https://unlicense.org/
 */

using System.Diagnostics;
using System.Text.Json;
using WebSocketSharp;
using CDP.Commands;
using CDP.Utils;

namespace CDP.Objects
{
    
    public class DOM
    {
        public DOM(Tab Parent, string Id)
        {
            this.Id = Id;
            this.Parent = Parent;
        }

        public string Id { get; }
        public Tab Parent { get; }

        public JsonDocument GetDocument(int depth, Boolean pierce, TimeSpan? TimeOut = null)
        {
            if (TimeOut == null) { TimeOut = TimeSpan.FromSeconds(10); }
            Stopwatch stopWatch = Stopwatch.StartNew();

            bool DOMNodeRetrieved = false; int commandId = 1;
            JsonDocument DOMDocument = JsonDocument.Parse("{}");
            
            using (var webSocket = new WebSocket("ws://localhost:9222/devtools/page/" + this.Id))
            {
                webSocket.OnMessage += (sender, e) =>
                {
                    CommandResult? result = JsonSerializer.Deserialize<CommandResult>(e.Data);
                    if (result == null) { throw new InvalidOperationException(); }
                    if (result.Id != commandId) { return; }
                    
                    DOMDocument = result.Result;
                    DOMNodeRetrieved = true;
                };

                webSocket.Connect();
                webSocket.Send(new DOMGetDocumentCommand(commandId, depth, pierce).ToString());

                while (DOMNodeRetrieved == false)
                {
                    if (stopWatch.Elapsed > TimeOut) { throw new TimeoutException(); }
                    Thread.Sleep(100);
                }

                webSocket.Close(); return DOMDocument;
            }
        }
    }
}