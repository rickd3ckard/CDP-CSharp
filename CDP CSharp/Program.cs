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
        if (browser == null) { return; }

        Tab defaultTab = browser.Tabs[0];

        defaultTab.NavigateTo(@"https://www.maisonsmoches.be/");
        defaultTab.DOM.GetDocument(-1, true);

        

        int nodeId = defaultTab.DOM.QuerySelector(1, "a[href=\"https://www.linkedin.com/company/nous-achetons-des-maisons-moches/\"]", TimeSpan.FromSeconds(10));
        Console.WriteLine(nodeId);

        BoxModel box = defaultTab.DOM.GetBoxModel(nodeId);
        Console.WriteLine(box.ToString());
        defaultTab.DOM.DispatchMouseEvent(box.Center, MouseButtonEnum.left);

        Thread.Sleep(3000);
        Console.WriteLine(defaultTab.LayoutViewport);
    }
}