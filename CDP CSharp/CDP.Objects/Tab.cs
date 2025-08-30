/* 
 * Public domain software, no restrictions. 
 * Released by rickd3ckard: https://github.com/rickd3ckard 
 * See: https://unlicense.org/
 */

using System.Diagnostics;
using System.Text.Json;
using System.Net.WebSockets;
using System.Text;
using CDP.Commands;
using CDP.Utils;

namespace CDP.Objects
{
    public class Tab
    {
        public Tab(Browser Browser, string Id)
        {
            this.Parent = Browser;
            this.Id = Id;
            this.DOM = new DOM(this, Id);
            this.LayoutViewport = Task.Run(() => GetLayoutMetrics()).Result;
        }

        public Browser Parent { get; }
        public String Id { get; }
        public DOM DOM { get; }
        public LayoutViewport LayoutViewport { get; }

        public async Task NavigateTo(string URL, TimeSpan? TimeOut = null)
        {
            if (TimeOut == null) { TimeOut = TimeSpan.FromSeconds(10); }
            Stopwatch stopWatch = Stopwatch.StartNew();

            LayoutViewport? layout = new LayoutViewport();
            using (ClientWebSocket webSocket = new ClientWebSocket())
            {
                Uri socketUri = new Uri("ws://localhost:9222/devtools/page/" + this.Id);
                await webSocket.ConnectAsync(socketUri, CancellationToken.None);
                await webSocket.SendAsync(new PageEnableCommand(1).Encode(),
                    WebSocketMessageType.Text, true, CancellationToken.None);
                await webSocket.SendAsync(new PageNavigateCommand(2, URL).Encode(),
                    WebSocketMessageType.Text, true, CancellationToken.None);

                byte[] responseBuffer = new byte[1024];
                StringBuilder responseBuilder = new StringBuilder();

                while (true)
                {
                    if (stopWatch.Elapsed > TimeOut) { throw new TimeoutException(); }
                    WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(responseBuffer), CancellationToken.None);
                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Done", CancellationToken.None);
                        break;
                    }

                    responseBuilder.Append(Encoding.UTF8.GetString(responseBuffer, 0, result.Count)); // remove two args?
                    Console.ForegroundColor = ConsoleColor.Yellow; Console.WriteLine(responseBuilder.ToString()); Console.ResetColor();
                    if (result.EndOfMessage == true)
                    {
                        string response = responseBuilder.ToString();
                        responseBuilder.Clear();

                        MessageEventData? commandResult = JsonSerializer.Deserialize<MessageEventData>(response);
                        if (commandResult == null) { throw new InvalidCastException(); }
                        if (commandResult.Method == "Page.frameStoppedLoading")
                        {
                            string? frameID = commandResult.Params["frameId"].RootElement.GetString();
                            if (frameID == null) { throw new InvalidCastException(); }
                            if (frameID == this.Id)
                            {
                                await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
                                break;
                            }
                        }
                    }
                }
            }
        }

        public async Task Focus() // could be bool to chekc success
        {
            using (HttpClient client = new HttpClient())
            {
                await client.GetAsync($@"http://localhost:9222/json/activate/{this.Id}");
            }
        }

        public async Task Close() // could be bool to chekc success
        {
            await this.Parent.CloseTab(this.Id);
        }

        public async Task<LayoutViewport> GetLayoutMetrics(TimeSpan? TimeOut = null)
        {
            if (TimeOut == null) { TimeOut = TimeSpan.FromSeconds(10); }
            Stopwatch stopWatch = Stopwatch.StartNew();

            int commandId = 1; LayoutViewport? layout = new LayoutViewport();
            using (ClientWebSocket webSocket = new ClientWebSocket())
            {
                Uri socketUri = new Uri("ws://localhost:9222/devtools/page/" + this.Id);
                await webSocket.ConnectAsync(socketUri, CancellationToken.None);
                await webSocket.SendAsync(new PageGetLayoutMetricsCommand(1).Encode(),
                    WebSocketMessageType.Text, true, CancellationToken.None);

                byte[] responseBuffer = new byte[1024];
                StringBuilder responseBuilder = new StringBuilder();

                while (true)
                {
                    if (stopWatch.Elapsed > TimeOut) { throw new TimeoutException(); }
                    WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(responseBuffer), CancellationToken.None);
                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Done", CancellationToken.None);
                        break;
                    }

                    responseBuilder.Append(Encoding.UTF8.GetString(responseBuffer, 0, result.Count)); // remove two args?
                    Console.ForegroundColor = ConsoleColor.Yellow; Console.WriteLine(responseBuilder.ToString()); Console.ResetColor();
                    if (result.EndOfMessage == true)
                    {
                        string response = responseBuilder.ToString();
                        responseBuilder.Clear();

                        CommandResult? commandResult = JsonSerializer.Deserialize<CommandResult>(response);
                        if (commandResult == null) { throw new InvalidCastException(); }
                        if (commandResult.Id == commandId)
                        {
                            layout = JsonSerializer.Deserialize<LayoutViewport>(commandResult.Result.RootElement.GetProperty("cssLayoutViewport").GetRawText());
                            if (layout == null) { throw new InvalidOperationException(); }
                            await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
                            break;
                        }
                    }
                }

                return layout;
            }
        }
    }
}