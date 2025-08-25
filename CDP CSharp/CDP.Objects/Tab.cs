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
            this.DOM = new DOM(this, Id);
            this.LayoutViewport = GetLayoutMetrics();
        }

        public Browser Parent { get; }
        public String Id { get; }
        public DOM DOM { get; }
        public LayoutViewport LayoutViewport { get; }

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

        public LayoutViewport GetLayoutMetrics(TimeSpan? TimeOut = null)
        {
            if (TimeOut == null) { TimeOut = TimeSpan.FromSeconds(10); }
            Stopwatch stopWatch = Stopwatch.StartNew();

            LayoutViewport? layout = new LayoutViewport();
            using (var webSocket = new WebSocket("ws://localhost:9222/devtools/page/" + this.Id))
            {
                bool commandCompleted = false;
                webSocket.OnMessage += (sender, e) =>
                {
                    CommandResult? result = JsonSerializer.Deserialize<CommandResult>(e.Data);
                    if (result == null) {throw new InvalidOperationException(); }                 
                    if (result.Id == 1)
                    {
                        Thread.Sleep(3000);
                        Console.WriteLine(result.Result.RootElement.GetRawText());
                        layout = JsonSerializer.Deserialize<LayoutViewport>(result.Result.RootElement.GetProperty("cssLayoutViewport").GetRawText());
                        if (layout == null) {throw new InvalidOperationException(); }
                        commandCompleted = true;
                    } 
                };

                webSocket.Connect();
                webSocket.Send(new PageGetLayoutMetricsCommand(1).ToString());

                while (commandCompleted == false)
                {
                    if (stopWatch.Elapsed > TimeOut) { throw new TimeoutException(); }
                    Thread.Sleep(100);
                }

                webSocket.Close(); return layout;
            }
        }
    }
}