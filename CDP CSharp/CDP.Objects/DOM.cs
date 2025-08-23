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
using System.Runtime.CompilerServices;

namespace CDP.Objects
{
    public class DOM
    {
        private JsonDocument? _document;

        public DOM(Tab Parent, string Id)
        {
            this.Id = Id;
            this.Parent = Parent;
            this.WebSocket = new WebSocket("ws://localhost:9222/devtools/page/" + Id);
            this.WebSocket.Connect();

            EventHandler<CloseEventArgs> closeEvent = (sender, e) => {
                _document = null;
                throw new InvalidOperationException();
            };

            this.WebSocket.OnClose += closeEvent;
        }

        public string Id { get; }
        public Tab Parent { get; }
        public JsonDocument? Document { get { return _document; } }
        private WebSocket WebSocket { get; }
        
        public JsonDocument GetDocument(int depth, Boolean pierce, TimeSpan? TimeOut = null) // return DOM to be able to chain GetDocument().QuerySelector()
        {
            if (TimeOut == null) { TimeOut = TimeSpan.FromSeconds(10); }
            Stopwatch stopWatch = Stopwatch.StartNew();

            bool DOMNodeRetrieved = false; int commandId = 1;
            JsonDocument DOMDocument = JsonDocument.Parse("{}");

            EventHandler<MessageEventArgs> handler = (sender, e) =>
            {
                CommandResult? result = JsonSerializer.Deserialize<CommandResult>(e.Data);
                if (result == null) { throw new InvalidOperationException(); }
                if (result.Id != commandId) { return; }

                DOMDocument = result.Result;
                DOMNodeRetrieved = true;
            };

            this.WebSocket.OnMessage += handler;
            this.WebSocket.Send(new DOMGetDocumentCommand(commandId, depth, pierce).ToString());
            while (DOMNodeRetrieved == false)
            {
                if (stopWatch.Elapsed > TimeOut) { throw new TimeoutException(); }
                Thread.Sleep(100);
            }

            this.WebSocket.OnMessage -= handler;
            _document = DOMDocument;  return DOMDocument;
        }

        public int QuerySelector(int NodeId, string Selector, TimeSpan? TimeOut = null) // return nodeObject (generic) with type param?
        {
            if (_document == null) { throw new NullReferenceException(); }

            if (TimeOut == null) { TimeOut = TimeSpan.FromSeconds(10); }
            Stopwatch stopWatch = Stopwatch.StartNew();
           
            int resultNodeId = -1;
            bool DOMQuerySelectorCompleted = false; int querSelectorCommandId = 1;

            EventHandler<MessageEventArgs> handler = (sender, e) => {
                CommandResult? result = JsonSerializer.Deserialize<CommandResult>(e.Data);
                if (result == null) { throw new InvalidCastException(); }
                if (result.Id != querSelectorCommandId) { return; }

                DOMQuerySelectorCompleted = true;
                resultNodeId = result.Result.RootElement.GetProperty("nodeId").GetInt32();
            };

            this.WebSocket.OnMessage += handler;
            this.WebSocket.Send(new DOMQuerySelectorCommad(querSelectorCommandId, NodeId, Selector).ToString());
            while (!DOMQuerySelectorCompleted)
            {
                if (stopWatch.Elapsed > TimeOut) { throw new TimeoutException(); }
                Thread.Sleep(100);
            }

            this.WebSocket.OnMessage -= handler;
            return resultNodeId;
        }

        public BoxModel GetBoxModel(int NodeId, TimeSpan? TimeOut = null)
        {
            if (_document == null) { throw new NullReferenceException(); }
            if (TimeOut == null) { TimeOut = TimeSpan.FromSeconds(10); }
            Stopwatch stopWatch = Stopwatch.StartNew();

            bool commandCompleted = false; int commandId = 1;
            JsonDocument resultDocument = JsonDocument.Parse("{}");
            EventHandler<MessageEventArgs> handler = (sender, e) => {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine(e.Data + "\n\t" ); Console.ResetColor();
                
                CommandResult? result = JsonSerializer.Deserialize<CommandResult>(e.Data);
                if (result == null) { throw new InvalidCastException(); }
                if (result.Id != commandId) { return; }
                resultDocument = result.Result; commandCompleted = true;
            };

            this.WebSocket.OnMessage += handler;
            this.WebSocket.Send(new DOMGetBoxModelCommand(1, NodeId).ToString());

            while (commandCompleted == false)
            {
                if (stopWatch.Elapsed > TimeOut) { throw new TimeoutException(); }
                Thread.Sleep(100);
            }

            return new BoxModel(resultDocument);
        }
    }
}