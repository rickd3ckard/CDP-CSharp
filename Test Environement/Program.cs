/* 
 * Public domain software, no restrictions. 
 * Released by rickd3ckard: https://github.com/rickd3ckard 
 * See: https://unlicense.org/
 */

using CDP.Objects;
using CDP.Utils;
using System.Text.Json;

class Program // test environement
{
    static async Task Main()
    {
        Browser? browser = await new Browser().Start();
        if (browser == null) { throw new InvalidOperationException(); }

        await browser.SetWindowsBound(WindowStateEnum.maximized);

        Tab defaultTab = browser.Tabs[0];

        await defaultTab.NavigateTo(@"https://www.ipi.be/agent-immobilier?location=4000&page=1");
        //defaultTab.DOM.GetDocument(-1, true);

        //int nodeId = defaultTab.DOM.QuerySelector(1, "button[id=\"CybotCookiebotDialogBodyButtonDecline\"]");

        //if (nodeId != 0)
        //{
        //    BoxModel box = defaultTab.DOM.GetBoxModel(nodeId);
        //    defaultTab.DOM.DispatchMouseEvent(box.Center, MouseButtonEnum.left);
        //}

        //Thread.Sleep(1000); // wait to be fully loaded

        //int[] nodeIds = defaultTab.DOM.QuerySelectorAll(1, "a[class=\"stretched-link outlined-link-hover\"]");
        //Console.WriteLine(nodeIds.Length);

        //foreach (int id in nodeIds)
        //{
        //    Node resultNode = defaultTab.DOM.DescribeNode(id);
        //    string? href = resultNode.GetAttributeValue("href");
        //    if (href == null) { throw new InvalidOperationException(); }

        //    await browser.OpenTab(href);
        //}

        //Thread.Sleep(10000);

        await browser.Close();
    }
}