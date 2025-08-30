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

        browser.SetWindowsBound(WindowStateEnum.maximized);

        Tab defaultTab = browser.Tabs[0];

        defaultTab.NavigateTo(@"https://www.ipi.be/agent-immobilier?location=4000&page=1");
        Thread.Sleep(2000);
        defaultTab.DOM.GetDocument(1, false);

        int nodeId = defaultTab.DOM.QuerySelector(1, "button[id=\"CybotCookiebotDialogBodyButtonDecline\"]");
        Console.WriteLine(nodeId);

        if (nodeId != 0)
        {
            BoxModel box = defaultTab.DOM.GetBoxModel(nodeId);
            defaultTab.DOM.DispatchMouseEvent(box.Center, MouseButtonEnum.left);
        }

        nodeId = defaultTab.DOM.QuerySelector(1, "a[class=\"stretched-link outlined-link-hover\"]");

        Node resultNode = defaultTab.DOM.DescribeNode(nodeId);
        string? href = resultNode.GetAttributeValue("href");
        if (href == null) { throw new InvalidOperationException(); }

        await browser.OpenTab(href);
       
        browser.Close();
    }
}

