/* 
 * Public domain software, no restrictions. 
 * Released by rickd3ckard: https://github.com/rickd3ckard 
 * See: https://unlicense.org/
 */

using System.Diagnostics;
using System.Text.Json;
using WebSocketSharp;
using CDP.Utils;
using CDP.Commands;

// https://chromedevtools.github.io/devtools-protocol/
// https://chromedevtools.github.io/devtools-protocol/tot/

namespace CDP.Objects
{
    // add public string for chrome path
    public class Browser
    {
        public Browser()
        {
            this.WebsocketTargets = new List<WebsocketTarget>();
            this.Tabs = new List<Tab>();
            this.Version = new BrowserVersionMetadata();
        }

        public List<WebsocketTarget> WebsocketTargets { get; set; }
        public List<Tab> Tabs { get; set; }
        public BrowserVersionMetadata Version { get; set; }

        public async Task<Browser?> Start()
        {
            string chromePath = @"C:\Program Files (x86)\Google\Chrome\Application\chrome.exe"; // input 
            string arguments = "--remote-debugging-port=9222 --no-sandbox --user-data-dir=\"C:\\temp\\chrome-debug\"";

            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = chromePath,
                Arguments = arguments,
                UseShellExecute = false
            };

            try { Process.Start(startInfo); }
            catch { return null; }
            
            await InitWebsocketTargets();
            await GetBrowserVersionMetadata();
            return this;
        }

        private async Task InitWebsocketTargets()
        {
            WebsocketTargets.Clear();

            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(@"http://localhost:9222/json");
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
                HttpResponseMessage response = await client.GetAsync(@"http://localhost:9222/json/version");
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
                HttpResponseMessage response = await client.GetAsync($@"http://localhost:9222/json/close/{Id}");

                this.Tabs.RemoveAll(tab => tab.Id == Id);
                this.WebsocketTargets.RemoveAll(tab => tab.Id == Id);
            }
        }

        public async Task<Tab> OpenTab(string URL)
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.PutAsync($@"http://localhost:9222/json/new?{Uri.EscapeDataString(URL)}", new StringContent(string.Empty));
                WebsocketTarget? newTabSocket = JsonSerializer.Deserialize<WebsocketTarget>(await response.Content.ReadAsStringAsync());
                if (newTabSocket == null) { throw new Exception(); }

                this.WebsocketTargets.Add(newTabSocket);
                Tab newTab = new Tab(this, newTabSocket.Id);
                this.Tabs.Add(new Tab(this, newTabSocket.Id));

                return newTab;
            }
        }

        public void Close()
        {
            using (WebSocket socket = new WebSocket(this.Version.WebSocketDebuggerUrl))
            {
                socket.Connect();
                socket.Send(new BrowserCloseCommand(1).ToString());
            }
        }
    }
}