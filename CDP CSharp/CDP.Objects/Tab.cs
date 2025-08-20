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
    public class Tab
    {
        public Tab(Browser Browser, string Id)
        {
            this.Parent = Browser;
            this.Id = Id;
        }

        public Browser Parent { get; }
        public String Id { get; }

        public void NavigateTo(string URL, TimeSpan? TimeOut = null)
        {
            if (TimeOut == null) { TimeOut = TimeSpan.FromSeconds(10); }
            Stopwatch stopWatch = Stopwatch.StartNew();

            using (var webSocket = new WebSocket("ws://localhost:9222/devtools/page/" + this.Id))
            {
                bool pageIsLoaded = false;
                webSocket.OnMessage += (sender, e) =>
                {
                    MessageEventData? eventData = JsonSerializer.Deserialize<MessageEventData>(e.Data);
                    if (eventData == null) { throw new InvalidOperationException(); }

                    if (eventData.Method == "Page.frameStoppedLoading")
                    {
                        string frameID = eventData.Params["frameId"].RootElement.GetString() ?? string.Empty;
                        if (frameID == this.Id) { pageIsLoaded = true; }
                    }
                };

                webSocket.Connect();
                webSocket.Send(new PageEnableCommand(1).ToString()); // Enable Page domain to get events
                webSocket.Send(new PageNavigateCommand(2, URL).ToString());

                while (pageIsLoaded == false)
                {
                    if (stopWatch.Elapsed > TimeOut) { throw new TimeoutException(); }
                    Thread.Sleep(100);
                }

                webSocket.Close();
            }
        }

        public void GetDOMDocument(int depth, Boolean pierce)
        {
            using (var webSocket = new WebSocket("ws://localhost:9222/devtools/page/" + this.Id))
            {
                webSocket.OnMessage += (sender, e) =>
                {
                    Console.WriteLine(e.Data);
                };

                var getDocumentCommand = new
                {
                    id = 1,
                    method = "DOM.getDocument",
                    @params = new
                    {
                        depth = -1,   
                        pierce = true
                    }
                };

                webSocket.Connect();
                webSocket.Send(JsonSerializer.Serialize(getDocumentCommand));

                Thread.Sleep(10000);
            }
        }

        public async Task Focus() // could be bool to chekc success
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync($@"http://localhost:9222/json/activate/{this.Id}");
            }
        }

        public async Task Close() // could be bool to chekc success
        {
            await this.Parent.CloseTab(this.Id);
        }

    }
}