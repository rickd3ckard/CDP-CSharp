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

        Tab defaultTab = browser.Tabs[0];

        await defaultTab.NavigateTo(@"https://www.ipi.be/agent-immobilier?location=4000&page=1");
        await defaultTab.DOM.GetDocument(-1, false, TimeSpan.FromSeconds(50));

        int nodeId = await defaultTab.DOM.QuerySelector(1, "button[id=\"CybotCookiebotDialogBodyButtonDecline\"]");

        if (nodeId != 0)
        {
            BoxModel box = await defaultTab.DOM.GetBoxModel(nodeId);
            await defaultTab.DOM.DispatchMouseEvent(box.Center, MouseButtonEnum.left);
        }

        Node? searchButton = await defaultTab.SelectNode("a[href=\"https://www.ipi.be/agent-immobilier-ipi/contactez-nous\"]");
        if (searchButton != null) {await searchButton.ScrollIntoView();}
    
        await browser.Close(); return;
    }
}