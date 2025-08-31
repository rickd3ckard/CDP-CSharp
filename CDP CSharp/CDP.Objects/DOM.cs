/* 
 * Public domain software, no restrictions. 
 * Released by rickd3ckard: https://github.com/rickd3ckard 
 * See: https://unlicense.org/
 */

using System.Diagnostics;
using System.Text.Json;
using System.Text;
using System.Net.WebSockets;
using CDP.Commands;
using CDP.Utils;

namespace CDP.Objects
{
    public class DOM
    {
        private JsonDocument? _document;

        public DOM(Tab Parent, string Id)
        {
            this.Id = Id;
            this.Parent = Parent;

            this.WebSocket = new ClientWebSocket();
            Task.Run(() =>  this.WebSocket.ConnectAsync(new Uri("ws://localhost:9222/devtools/page/" + Id), CancellationToken.None));
        }

        public string Id { get; }
        public Tab Parent { get; }
        public JsonDocument? Document { get { return _document; } }
        private ClientWebSocket WebSocket { get; }
        
        public async Task<JsonDocument> GetDocument(int depth, Boolean pierce, TimeSpan? TimeOut = null) // return DOM to be able to chain GetDocument().QuerySelector()
        {
            if (TimeOut == null) { TimeOut = TimeSpan.FromSeconds(10); }
            Stopwatch stopWatch = Stopwatch.StartNew();

            int commandId = 1; JsonDocument? DOMDocument = JsonDocument.Parse("{}");
            await this.WebSocket.SendAsync(new DOMGetDocumentCommand(commandId, depth, pierce).Encode(),
                WebSocketMessageType.Text, true, CancellationToken.None);

            byte[] responseBuffer = new byte[1024];
            StringBuilder responseBuilder = new StringBuilder();

            while (true)
            {
                if (stopWatch.Elapsed > TimeOut) { throw new TimeoutException(); }
                WebSocketReceiveResult result = await this.WebSocket.ReceiveAsync(new ArraySegment<byte>(responseBuffer), CancellationToken.None);

                responseBuilder.Append(Encoding.UTF8.GetString(responseBuffer, 0, result.Count)); // remove two args?
                //Console.ForegroundColor = ConsoleColor.Yellow; Console.WriteLine(responseBuilder.ToString()); Console.ResetColor();
                if (result.EndOfMessage == true)
                {
                    string response = responseBuilder.ToString();
                    responseBuilder.Clear();

                    CommandResult? commandResult = JsonSerializer.Deserialize<CommandResult>(response);
                    if (commandResult == null) { throw new InvalidOperationException(); }
                    if (commandResult.Id != commandId) { continue; }
                    DOMDocument = commandResult.Result;
                    _document = DOMDocument; return DOMDocument;
                }
            }
        }

        public async Task<int> QuerySelector(int NodeId, string Selector, TimeSpan? TimeOut = null) // return nodeObject (generic) with type param?
        {
            if (_document == null) { throw new NullReferenceException(); }
            if (TimeOut == null) { TimeOut = TimeSpan.FromSeconds(10); }
            Stopwatch stopWatch = Stopwatch.StartNew();

            int commandId = 1;
            await this.WebSocket.SendAsync(new DOMQuerySelectorCommand(commandId, NodeId, Selector).Encode(),
                WebSocketMessageType.Text, true, CancellationToken.None);

            byte[] responseBuffer = new byte[1024];
            StringBuilder responseBuilder = new StringBuilder();

            while (true)
            {
                if (stopWatch.Elapsed > TimeOut) { throw new TimeoutException(); }
                WebSocketReceiveResult result = await this.WebSocket.ReceiveAsync(new ArraySegment<byte>(responseBuffer), CancellationToken.None);

                responseBuilder.Append(Encoding.UTF8.GetString(responseBuffer, 0, result.Count)); // remove two args?
                // Console.ForegroundColor = ConsoleColor.Yellow; Console.WriteLine(responseBuilder.ToString()); Console.ResetColor();
                if (result.EndOfMessage == true)
                {
                    string response = responseBuilder.ToString();
                    responseBuilder.Clear();

                    CommandResult? commandResult = JsonSerializer.Deserialize<CommandResult>(response);
                    if (commandResult == null) { throw new InvalidOperationException(); }
                    if (commandResult.Id != commandId) { continue; }
                    return commandResult.Result.RootElement.GetProperty("nodeId").GetInt32();
               }
            }
        }

        public async Task<int[]> QuerySelectorAll(int NodeId, string Selector, TimeSpan? TimeOut = null)
        {
            if (_document == null) { throw new NullReferenceException(); }
            if (TimeOut == null) { TimeOut = TimeSpan.FromSeconds(10); }
            Stopwatch stopWatch = Stopwatch.StartNew();

            int commandId = 1;
            await this.WebSocket.SendAsync(new DOMQuerySelectorAllCommand(commandId, NodeId, Selector).Encode(),
                WebSocketMessageType.Text, true, CancellationToken.None);

            byte[] responseBuffer = new byte[1024];
            StringBuilder responseBuilder = new StringBuilder();

            while (true)
            {
                if (stopWatch.Elapsed > TimeOut) { throw new TimeoutException(); }
                WebSocketReceiveResult result = await this.WebSocket.ReceiveAsync(new ArraySegment<byte>(responseBuffer), CancellationToken.None);

                responseBuilder.Append(Encoding.UTF8.GetString(responseBuffer, 0, result.Count)); // remove two args?
                // Console.ForegroundColor = ConsoleColor.Yellow; Console.WriteLine(responseBuilder.ToString()); Console.ResetColor();
                if (result.EndOfMessage == true)
                {
                    string response = responseBuilder.ToString();
                    responseBuilder.Clear();

                    CommandResult? commandResult = JsonSerializer.Deserialize<CommandResult>(response);
                    if (commandResult == null) { throw new InvalidOperationException(); }
                    if (commandResult.Id != commandId) { continue; }
                    string rawNodeIds = commandResult.Result.RootElement.GetProperty("nodeIds").GetRawText();
                    int[]? nodeIds = JsonSerializer.Deserialize<int[]>(rawNodeIds);
                    if (nodeIds == null) { throw new InvalidCastException(); }
                    return nodeIds;
                }
            }
        }

        public async Task<BoxModel> GetBoxModel(int NodeId, TimeSpan? TimeOut = null)
        {
            if (_document == null) { throw new NullReferenceException(); }
            if (TimeOut == null) { TimeOut = TimeSpan.FromSeconds(10); }
            Stopwatch stopWatch = Stopwatch.StartNew();

            int commandId = 1;
            await this.WebSocket.SendAsync(new DOMGetBoxModelCommand(commandId, NodeId).Encode(),
                WebSocketMessageType.Text, true, CancellationToken.None);

            byte[] responseBuffer = new byte[1024];
            StringBuilder responseBuilder = new StringBuilder();

            while (true)
            {
                if (stopWatch.Elapsed > TimeOut) { throw new TimeoutException(); }
                WebSocketReceiveResult result = await this.WebSocket.ReceiveAsync(new ArraySegment<byte>(responseBuffer), CancellationToken.None);

                responseBuilder.Append(Encoding.UTF8.GetString(responseBuffer, 0, result.Count)); // remove two args?
                // Console.ForegroundColor = ConsoleColor.Yellow; Console.WriteLine(responseBuilder.ToString()); Console.ResetColor();
                if (result.EndOfMessage == true)
                {
                    string response = responseBuilder.ToString();
                    responseBuilder.Clear();

                    CommandResult? commandResult = JsonSerializer.Deserialize<CommandResult>(response);
                    if (commandResult == null) { throw new InvalidOperationException(); }
                    if (commandResult.Id != commandId) { continue; }
                    return new BoxModel(commandResult.Result);
                }
            }
        }

        public async Task DispatchMouseEvent(Point MousePosition, MouseButtonEnum MouseButton, TimeSpan? TimeOut = null)
        {
            if (_document == null) { throw new NullReferenceException(); }
            if (TimeOut == null) { TimeOut = TimeSpan.FromSeconds(10); }
            Stopwatch stopWatch = Stopwatch.StartNew();

            int pressCommandId = 1; int releaseCommandId = 2;
            await this.WebSocket.SendAsync(new InputDispatchMouseEventCommand(pressCommandId, MouseButton, MouseEventTypeEnum.mousePressed, MousePosition).Encode(),
                WebSocketMessageType.Text, true, CancellationToken.None);
            await this.WebSocket.SendAsync(new InputDispatchMouseEventCommand(releaseCommandId, MouseButton, MouseEventTypeEnum.mouseReleased, MousePosition).Encode(),
                WebSocketMessageType.Text, true, CancellationToken.None);

            byte[] responseBuffer = new byte[1024];
            StringBuilder responseBuilder = new StringBuilder();

            bool pressCommandCompleted = false; bool releaseCommandCompleted = false;
            while (pressCommandCompleted == false || releaseCommandCompleted == false)
            {
                if (stopWatch.Elapsed > TimeOut) { throw new TimeoutException(); }
                WebSocketReceiveResult result = await this.WebSocket.ReceiveAsync(new ArraySegment<byte>(responseBuffer), CancellationToken.None);

                responseBuilder.Append(Encoding.UTF8.GetString(responseBuffer, 0, result.Count)); // remove two args?
                // Console.ForegroundColor = ConsoleColor.Yellow; Console.WriteLine(responseBuilder.ToString()); Console.ResetColor();
                if (result.EndOfMessage == true)
                {
                    string response = responseBuilder.ToString();
                    responseBuilder.Clear();

                    CommandResult? commandResult = JsonSerializer.Deserialize<CommandResult>(response);
                    if (commandResult == null) { throw new InvalidOperationException(); }
                    if (commandResult.Id == pressCommandId) { pressCommandCompleted = true; }
                    if (commandResult.Id == releaseCommandId) { releaseCommandCompleted = true; }
                }
            }
        }

        public async Task ScrollIntoViewIfNeeded(int NodeId, TimeSpan? TimeOut = null) 
        {
            if (_document == null) { throw new NullReferenceException(); }
            if (TimeOut == null) { TimeOut = TimeSpan.FromSeconds(10); }
            Stopwatch stopWatch = Stopwatch.StartNew();

            int commandId = 1;
            await this.WebSocket.SendAsync(new DOMScrollIntoViewIfNeeded(commandId, NodeId).Encode(),
                WebSocketMessageType.Text, true, CancellationToken.None);

            byte[] responseBuffer = new byte[1024];
            StringBuilder responseBuilder = new StringBuilder();

            while (true)
            {
                if (stopWatch.Elapsed > TimeOut) { throw new TimeoutException(); }
                WebSocketReceiveResult result = await this.WebSocket.ReceiveAsync(new ArraySegment<byte>(responseBuffer), CancellationToken.None);

                responseBuilder.Append(Encoding.UTF8.GetString(responseBuffer, 0, result.Count)); // remove two args?
                // Console.ForegroundColor = ConsoleColor.Yellow; Console.WriteLine(responseBuilder.ToString()); Console.ResetColor();
                if (result.EndOfMessage == true)
                {
                    string response = responseBuilder.ToString();
                    responseBuilder.Clear();

                    CommandResult? commandResult = JsonSerializer.Deserialize<CommandResult>(response);
                    if (commandResult == null) { throw new InvalidOperationException(); }
                    if (commandResult.Id == commandId) { return; }
                }
            }
        }

        public async Task DispatchKeyEvent(Char Key, TimeSpan? TimeOut = null)
        {
            if (_document == null) { throw new NullReferenceException(); }
            if (TimeOut == null) { TimeOut = TimeSpan.FromSeconds(10); }
            Stopwatch stopWatch = Stopwatch.StartNew();

            int pressCommandId = 1; int releaseCommandId = 2;
            await this.WebSocket.SendAsync(new InputDispatchKeyEventCommand(pressCommandId, KeyEventTypeEnum.keyDown, Key).Encode(),
                WebSocketMessageType.Text, true, CancellationToken.None);
            await this.WebSocket.SendAsync(new InputDispatchKeyEventCommand(releaseCommandId, KeyEventTypeEnum.keyUp, Key).Encode(),
                WebSocketMessageType.Text, true, CancellationToken.None);

            byte[] responseBuffer = new byte[1024];
            StringBuilder responseBuilder = new StringBuilder();

            bool pressCommandCompleted = false; bool releaseCommandCompleted = false;
            while (pressCommandCompleted == false || releaseCommandCompleted == false)
            {
                if (stopWatch.Elapsed > TimeOut) { throw new TimeoutException(); }
                WebSocketReceiveResult result = await this.WebSocket.ReceiveAsync(new ArraySegment<byte>(responseBuffer), CancellationToken.None);

                responseBuilder.Append(Encoding.UTF8.GetString(responseBuffer, 0, result.Count)); // remove two args?
                // Console.ForegroundColor = ConsoleColor.Yellow; Console.WriteLine(responseBuilder.ToString()); Console.ResetColor();
                if (result.EndOfMessage == true)
                {
                    string response = responseBuilder.ToString();
                    responseBuilder.Clear();

                    CommandResult? commandResult = JsonSerializer.Deserialize<CommandResult>(response);
                    if (commandResult == null) { throw new InvalidOperationException(); }
                    if (commandResult.Id == pressCommandId) { pressCommandCompleted = true; }
                    if (commandResult.Id == releaseCommandId) { releaseCommandCompleted = true; }
                }
            }
        }

        public async Task<Node> DescribeNode(int NodeId, TimeSpan? TimeOut = null)
        {
            if (_document == null) { throw new NullReferenceException(); }
            if (TimeOut == null) { TimeOut = TimeSpan.FromSeconds(10); }
            Stopwatch stopWatch = Stopwatch.StartNew();

            int commandId = 1;
            await this.WebSocket.SendAsync(new DOMDescribeNodeCommand(commandId, NodeId).Encode(),
                WebSocketMessageType.Text, true, CancellationToken.None);

            byte[] responseBuffer = new byte[1024];
            StringBuilder responseBuilder = new StringBuilder();

            while (true)
            {
                if (stopWatch.Elapsed > TimeOut) { throw new TimeoutException(); }
                WebSocketReceiveResult result = await this.WebSocket.ReceiveAsync(new ArraySegment<byte>(responseBuffer), CancellationToken.None);

                responseBuilder.Append(Encoding.UTF8.GetString(responseBuffer, 0, result.Count)); // remove two args?
                // Console.ForegroundColor = ConsoleColor.Yellow; Console.WriteLine(responseBuilder.ToString()); Console.ResetColor();
                if (result.EndOfMessage == true)
                {
                    string response = responseBuilder.ToString();
                    responseBuilder.Clear();

                    CommandResult? commandResult = JsonSerializer.Deserialize<CommandResult>(response);
                    if (commandResult == null) { throw new InvalidOperationException(); }
                    if (commandResult.Id != commandId) { continue; }

                    JsonElement nodeElement = commandResult.Result.RootElement.GetProperty("node");
                    Node? nodeDescription = JsonSerializer.Deserialize<Node>(nodeElement.GetRawText());
                    if (nodeDescription == null) { throw new InvalidCastException(); }
                    nodeDescription.DOM = this;
                    return nodeDescription;
                }
            }
        }

        public async Task WriteText(string Text) // could be optimised
        {
            foreach (Char key in Text.ToCharArray())
            {
                await this.DispatchKeyEvent(key);
            }
        }
    }
}