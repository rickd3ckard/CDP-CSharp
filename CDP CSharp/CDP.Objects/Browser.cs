/* 
 * Public domain software, no restrictions. 
 * Released by rickd3ckard: https://github.com/rickd3ckard 
 * See: https://unlicense.org/
 */

using System.Diagnostics;
using System.Text.Json;
using CDP.Utils;

namespace CDP.Objects
{
    // add public string for chrome path
    public class Browser
    {
        public Browser()
        {
            this.WebsocketTargets = new List<WebsocketTarget>();
            this.Tabs = new List<Tab>();
        }

        public List<WebsocketTarget> WebsocketTargets { get; set; }
        public List<Tab> Tabs { get; set; }

        public async Task<Browser?> Start()
        {
            string chromePath = @"C:\Program Files (x86)\Google\Chrome\Application\chrome.exe";
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
            return this;
        }

        private async Task InitWebsocketTargets()
        {
            WebsocketTargets.Clear();

            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync("http://localhost:9222/json");
                string jsonResponse = await response.Content.ReadAsStringAsync();

                this.WebsocketTargets = JsonSerializer.Deserialize<List<WebsocketTarget>>(jsonResponse) ?? new List<WebsocketTarget>();

                foreach (WebsocketTarget socket in this.WebsocketTargets)
                {
                    if (socket.Type == "page") { this.Tabs.Add(new Tab(this, socket.Id)); }
                }
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
    }
}