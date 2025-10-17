/* 
 * Public domain software, no restrictions. 
 * Released by rickd3ckard: https://github.com/rickd3ckard 
 * See: https://unlicense.org/
 */

using System.Diagnostics;
using System.Text.Json;
using System.Net.WebSockets;
using System.Text;
using CDP.Utils;
using CDP.Commands;

// https://chromedevtools.github.io/devtools-protocol/
// https://chromedevtools.github.io/devtools-protocol/tot/

namespace CDP.Objects
{
    // add public string for chrome path
    public class Browser
    {
        public Browser(string ChromeExectutablePath, bool Headless = false, int Port = 9222)
        {
            if (string.IsNullOrWhiteSpace(ChromeExectutablePath)) { throw new ArgumentNullException(); }

            this.ChromeExectutablePath = ChromeExectutablePath;
            this.Headless = Headless;
            this.WebsocketTargets = new List<WebsocketTarget>();
            this.Tabs = new List<Tab>();
            this.Version = new BrowserVersionMetadata();
            this.Port = Port;
        }

        public List<WebsocketTarget> WebsocketTargets { get; set; }
        public List<Tab> Tabs { get; set; }
        public BrowserVersionMetadata Version { get; set; }
        public bool CloseRequested { get; set; }
        public int WindowId { get; set; }
        public bool Headless { get; }
        public string ChromeExectutablePath { get; }
        public int Port { get; }

        public async Task<Browser?> Start()
        {
            string arguments = Headless 
                ? $"--remote-debugging-port={this.Port}  --headless --user-data-dir=\"C:\\temp\\chrome-debug\\{this.Port}\""
                : $"--remote-debugging-port={this.Port}  --user-data-dir=\"C:\\temp\\chrome-debug\\{this.Port}\"";

            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = this.ChromeExectutablePath,
                Arguments = arguments,
                UseShellExecute = false
            };

            try { Process.Start(startInfo); }
            catch { return null; }
            
            await InitWebsocketTargets();
            await GetBrowserVersionMetadata();
            this.WindowId = await GetWindowForTarget(this.Tabs[0].Id);
            return this;
        }

        private async Task InitWebsocketTargets()
        {
            WebsocketTargets.Clear();

            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(@$"http://localhost:{this.Port}/json");
                string jsonResponse = await response.Content.ReadAsStringAsync();

                this.WebsocketTargets = JsonSerializer.Deserialize<List<WebsocketTarget>>(jsonResponse) ?? new List<WebsocketTarget>();

                foreach (WebsocketTarget socket in this.WebsocketTargets)
                {
                    if (socket.Type == "page") { this.Tabs.Add(new Tab(this, socket.Id)); }
                }
            }
        }

        private async Task GetBrowserVersionMetadata()
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(@$"http://localhost:{this.Port}/json/version");
                string jsonReponse = await response.Content.ReadAsStringAsync();

                BrowserVersionMetadata? version = JsonSerializer.Deserialize<BrowserVersionMetadata>(jsonReponse);
                if (version == null) { throw new InvalidCastException(); }
                this.Version = version;
            }
        }

        public async Task CloseTab(String Id)
        {
            using (HttpClient client = new HttpClient())
            {
                // we could add something like if the tab count is == 1 open a void tab first then close so browser doesn't shut down
                HttpResponseMessage response = await client.GetAsync($@"http://localhost:{this.Port}/json/close/{Id}");

                this.Tabs.RemoveAll(tab => tab != null && tab.Id == Id);
                this.Tabs.RemoveAll(tab => tab == null);
                this.WebsocketTargets.RemoveAll(tab => tab != null && tab.Id == Id);
                this.WebsocketTargets.RemoveAll(tab => tab == null);
            }
        }

        public async Task<Tab> OpenTab(string URL)
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.PutAsync($@"http://localhost:{this.Port}/json/new?{Uri.EscapeDataString(URL)}", new StringContent(string.Empty));
                WebsocketTarget? newTabSocket = JsonSerializer.Deserialize<WebsocketTarget>(await response.Content.ReadAsStringAsync());
                if (newTabSocket == null) { throw new InvalidCastException(); }

                this.WebsocketTargets.Add(newTabSocket);
                Tab newTab = new Tab(this, newTabSocket.Id);
                this.Tabs.Add(new Tab(this, newTabSocket.Id));

                return newTab;
            }
        }

        private async Task<int> GetWindowForTarget(string WebsocketId, TimeSpan? TimeOut = null)
        {
            if (this.Version.WebSocketDebuggerUrl == null) { throw new InvalidOperationException(); }
            if (TimeOut == null) { TimeOut = TimeSpan.FromSeconds(10); }
            Stopwatch stopWatch = Stopwatch.StartNew();

            int commandId = 1; int windowId = -1;
            using (ClientWebSocket webSocket = new ClientWebSocket())
            {
                Uri socketUri = new Uri($"ws://localhost:{this.Port}/devtools/page/" + WebsocketId);
                await webSocket.ConnectAsync(socketUri, CancellationToken.None);
                await webSocket.SendAsync(new BrowserGetWindowForTarget(commandId).Encode(),
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
                    // Console.ForegroundColor = ConsoleColor.Yellow; Console.WriteLine(responseBuilder.ToString()); Console.ResetColor();
                    if (result.EndOfMessage == true)
                    {
                        string response = responseBuilder.ToString();
                        responseBuilder.Clear();

                        CommandResult? commandResult = JsonSerializer.Deserialize<CommandResult>(response);
                        if (commandResult == null) { throw new InvalidCastException(); }
                        if (commandResult.Id == commandId)
                        {
                            windowId = commandResult.Result.RootElement.GetProperty("windowId").GetInt32();
                            await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
                            break;
                        }
                    }
                }

                return windowId;
            }
        }

        public async Task SetWindowsBound(WindowStateEnum WindowState, TimeSpan? TimeOut = null)
        {
            if (this.Version.WebSocketDebuggerUrl == null) { throw new InvalidOperationException(); }
            if (TimeOut == null) { TimeOut = TimeSpan.FromSeconds(10); }
            Stopwatch stopWatch = Stopwatch.StartNew();

            int commandId = 1;      
            using (ClientWebSocket webSocket = new ClientWebSocket())
            {
                Uri socketUri = new Uri(this.Version.WebSocketDebuggerUrl);
                await webSocket.ConnectAsync(socketUri, CancellationToken.None);
                await webSocket.SendAsync(new BrowsetSetWindowStateCommand(commandId, this.WindowId, WindowState).Encode(), 
                    WebSocketMessageType.Text, true, CancellationToken.None);

                byte[] responseBuffer = new byte[1024];
                StringBuilder responseBuilder = new StringBuilder();

                while (true)
                {
                    if (stopWatch.Elapsed > TimeOut) { throw new TimeoutException(); }
                    WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(responseBuffer), CancellationToken.None);
                    if (result.MessageType == WebSocketMessageType.Close) { 
                        await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Done", CancellationToken.None);
                        break;
                    }

                    responseBuilder.Append(Encoding.UTF8.GetString(responseBuffer, 0, result.Count)); // remove two args?
                    //Console.ForegroundColor = ConsoleColor.Yellow; Console.WriteLine(responseBuilder.ToString()); Console.ResetColor();
                    if (result.EndOfMessage == true)
                    {
                        string response = responseBuilder.ToString();
                        responseBuilder.Clear();

                        CommandResult? commandResult = JsonSerializer.Deserialize<CommandResult>(response);
                        if (commandResult == null) { throw new InvalidCastException(); }
                        if (commandResult.Id == commandId)
                        {
                            await Task.Delay(1000);
                            await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
                            break;
                        }
                    }
                }
            }
        }
        
        public async Task Close()
        {
            if (this.Version.WebSocketDebuggerUrl == null) { throw new InvalidOperationException(); }       
            this.CloseRequested = true;
         
            using (ClientWebSocket webSocket = new ClientWebSocket())
            {
                Uri socketUri = new Uri(this.Version.WebSocketDebuggerUrl);
                await webSocket.ConnectAsync(socketUri, CancellationToken.None);
                await webSocket.SendAsync(new BrowserCloseCommand(1).Encode(), WebSocketMessageType.Text, true, CancellationToken.None);
                await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
            }
        }
    }
}