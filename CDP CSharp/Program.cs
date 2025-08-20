using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using WebSocketSharp;

class Program
{
    static async Task Main()
    {
        Browser? browser = await new Browser().Start();
        if (browser == null) { return; }

        BrowserTab defaultTab = browser.Tabs[0];

        defaultTab.NavigateTo(@"https://www.maisonsmoches.be/contact/");

        defaultTab.GetDOMDocument(-1,false);

        defaultTab.NavigateTo(@"https://www.youtube.com/watch?v=7wxDn56hF8Y");
        defaultTab.NavigateTo(@"https://www.canva.com/design/DAGwio5xcFM/Z7w7nbe_syCGXkRgiYLsmw/edit");
    }
}

public class Browser
{
    public Browser() 
    {
        this.WebsocketTargets = new List<WebsocketTarget>();
        this.Tabs = new List<BrowserTab>();
    }

    public List<WebsocketTarget> WebsocketTargets { get; set; }
    public List<BrowserTab> Tabs { get; set; }

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
                if (socket.Type == "page") { this.Tabs.Add(new BrowserTab(this, socket.Id)); }
            }
        }
    } 

    public async Task CloseTab(String Id)
    {
        using (HttpClient client = new HttpClient())
        {
            // we could add something like if the tab count is == 1 open a void tab first then close so browser doesn't shut down
            HttpResponseMessage response = await client.GetAsync($@"http://localhost:9222/json/close/{Id}"); //Console.WriteLine(response.ToString());

            this.Tabs.RemoveAll(tab => tab.Id == Id);
            this.WebsocketTargets.RemoveAll(tab => tab.Id == Id);
        }
    }

    public async Task<BrowserTab> OpenTab(string URL)
    {
        using (HttpClient client = new HttpClient())
        {
            HttpResponseMessage response = await client.PutAsync($@"http://localhost:9222/json/new?{Uri.EscapeDataString(URL)}", new StringContent(string.Empty));
            //Console.WriteLine(response.ToString());
            WebsocketTarget? newTabSocket = JsonSerializer.Deserialize<WebsocketTarget>(await response.Content.ReadAsStringAsync());
            if (newTabSocket == null) { throw new Exception(); }

            this.WebsocketTargets.Add(newTabSocket);
            BrowserTab newTab = new BrowserTab(this, newTabSocket.Id);
            this.Tabs.Add(new BrowserTab(this, newTabSocket.Id)); 
            
            return newTab;
        }
    }
}

public class BrowserTab
{
    public BrowserTab(Browser Browser, string Id)
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
                MessageEventData? eventData =  JsonSerializer.Deserialize<MessageEventData>(e.Data);
                if (eventData == null) { throw new InvalidOperationException(); }

                if (eventData.Method == "Page.frameStoppedLoading") 
                {
                    string frameID = eventData.Params["frameId"].RootElement.GetString() ?? string.Empty ;
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
                    depth = -1,   // Gets full tree (optional)
                    pierce = true // Includes shadow DOM (optional)
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

public class DomGetDocumentCommand()
{

}

public class PageNavigateCommand
{
    public PageNavigateCommand(int Id, string URL)
    {
        this.Id = Id;
        this.Method = "Page.navigate";
        this.Params = new Dictionary<string, string>();
        this.Params.Add("url", URL);
    }

    public int Id { get; }
    public string Method { get; }
    public Dictionary<string, string> Params { get; }

    public override string ToString()
    {
        JsonSerializerOptions options = new JsonSerializerOptions();
        options.WriteIndented = true;
        options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;

        return JsonSerializer.Serialize(this, options);
    }
}

public class PageEnableCommand
{
    public PageEnableCommand(int Id)
    {
        this.Id = Id;
        this.Method = "Page.enable";
    }
    
    public int Id { get; }
    public string Method { get; }

    public override string ToString()
    {
        JsonSerializerOptions options = new JsonSerializerOptions();
        options.WriteIndented = true;
        options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;

        return JsonSerializer.Serialize(this, options);
    }
}

public class MessageEventData
{
    public MessageEventData()
    {
        this.Method = string.Empty;
        this.Params = new Dictionary<string, JsonDocument>();
    }

    [JsonPropertyName("method")]
    public string Method { get; set; }

    [JsonPropertyName("params")]
    public Dictionary<string, JsonDocument> Params { get; set; }

    public override string ToString()
    {
        JsonSerializerOptions options = new JsonSerializerOptions();
        options.WriteIndented = true;

        return JsonSerializer.Serialize(this, options);
    }
}

public class WebsocketTarget
{
    public WebsocketTarget() 
    {
        this.Description = string.Empty;
        this.DevtoolsFrontendUrl = string.Empty;
        this.Id = string.Empty;
        this.Title = string.Empty;
        this.Type = string.Empty;
        this.URL = string.Empty;
        this.WebSocketDebuggerURL = string.Empty;
    }

    [JsonPropertyName("description")]
    public string Description { get; set; }

    [JsonPropertyName("devtoolsFrontendUrl")]
    public string DevtoolsFrontendUrl { get; set; }

    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("url")]
    public string URL { get; set; }

    [JsonPropertyName("webSocketDebuggerUrl")]
    public string WebSocketDebuggerURL { get; set; }

    public override string ToString()
    {
        JsonSerializerOptions options = new JsonSerializerOptions();
        options.WriteIndented = true;

        return JsonSerializer.Serialize(this, options) ;
    }
}
