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

        browser.SetWindowsBound(WindowStateEnum.maximized);

        Tab defaultTab = browser.Tabs[0];
        
        defaultTab.NavigateTo(@"https://www.maisonsmoches.be/contact/");
        defaultTab.DOM.GetDocument(-1, true);
       
        int nodeId = defaultTab.DOM.QuerySelector(1, "a[href=\"https://www.maisonsmoches.be/vie-privee/\"]", TimeSpan.FromSeconds(10));
        defaultTab.DOM.ScrollIntoViewIfNeeded(nodeId);

        Node resultNode = defaultTab.DOM.DescribeNode(nodeId);
        Console.WriteLine(resultNode);

        BoxModel box = defaultTab.DOM.GetBoxModel(nodeId);
        defaultTab.DOM.DispatchMouseEvent(box.Center, MouseButtonEnum.left);

        Thread.Sleep(2000);

        defaultTab.DOM.GetDocument(-1, true);
        nodeId = defaultTab.DOM.QuerySelector(1, "h1[class=\"big-title big-title--tertiary lazyScroll lazyScrollView\"]");
        resultNode = defaultTab.DOM.DescribeNode(nodeId);
        Console.WriteLine(resultNode);

        defaultTab.DOM.WriteText("Hello, world!");
        browser.Close();
    }
}
