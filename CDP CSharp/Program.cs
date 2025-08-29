/* 
 * Public domain software, no restrictions. 
 * Released by rickd3ckard: https://github.com/rickd3ckard 
 * See: https://unlicense.org/
 */

using CDP.Objects;
using CDP.Utils;
using System.Dynamic;

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
       
        int nodeId = defaultTab.DOM.QuerySelector(1, "input[class=\"wpcf7-form-control wpcf7-text wpcf7-validates-as-required\"]", TimeSpan.FromSeconds(10));
        defaultTab.DOM.ScrollIntoViewIfNeeded(nodeId);

        Console.WriteLine(defaultTab.DOM.DescribeNode(nodeId).RootElement.GetRawText());

        BoxModel box = defaultTab.DOM.GetBoxModel(nodeId);
        defaultTab.DOM.DispatchMouseEvent(box.Center, MouseButtonEnum.left);


        defaultTab.DOM.WriteText("Hello, world!");
        browser.Close();
    }
}
