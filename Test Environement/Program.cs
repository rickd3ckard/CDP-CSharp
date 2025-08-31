/* 
 * Public domain software, no restrictions. 
 * Released by rickd3ckard: https://github.com/rickd3ckard 
 * See: https://unlicense.org/
 */

using CDP.Objects;
using CDP.Utils;

class Program // test environement
{
    static async Task Main()
    {
        Browser? browser = await new Browser().Start();
        if (browser == null) { throw new InvalidOperationException(); }
        await browser.SetWindowsBound(WindowStateEnum.maximized);

        Tab tab = browser.Tabs[0];
        await tab.NavigateTo(@"https://www.ipi.be/agent-immobilier?location=4000&page=1"); Thread.Sleep(2000); // is executing some javascript
        await tab.DOM.GetDocument(1, true, TimeSpan.FromSeconds(50));

        Node? declineCookieButton = await tab.SelectNode("button[id=\"CybotCookiebotDialogBodyButtonDecline\"]");
        if (declineCookieButton != null) { await declineCookieButton.Click(MouseButtonEnum.left, true); Thread.Sleep(500); }

        Node[]? agentCards = await tab.SelectNodes("a[class=\"stretched-link outlined-link-hover\"]");
        if (agentCards == null) { return; }

        List<Task> taskList = new List<Task>();
        List<string> names = new List<string>();

        foreach (Node agentCard in agentCards)
        {
            string? href = agentCard.GetAttributeValue("href");
            if (href != null)
            {
                taskList.Add(Task.Run(async () => {
                    Tab agentTab = await browser.OpenTab(href);
                    await agentTab.DOM.GetDocument(1, true);
                    Node? agentName = await agentTab.SelectNode("h2[class=\"fw-bold fs-2xl mb-0\"]");
                    if (agentName != null) { names.Add(agentName.GetText() ?? string.Empty); }
                    await agentTab.Close(); await tab.Focus();
                }));
            }
            Thread.Sleep(100);
        }

        await Task.WhenAll(taskList);
        names.ForEach(Console.WriteLine);
        await browser.Close(); return;
    }
}