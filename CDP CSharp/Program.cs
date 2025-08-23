/* 
 * Public domain software, no restrictions. 
 * Released by rickd3ckard: https://github.com/rickd3ckard 
 * See: https://unlicense.org/
 */

using CDP.Objects;
using System.Diagnostics;

class Program // test environement
{
    static async Task Main()
    {
        Browser? browser = await new Browser().Start();
        if (browser == null) { return; }

        Tab defaultTab = browser.Tabs[0];

        defaultTab.NavigateTo(@"https://www.maisonsmoches.be/contact/");
        defaultTab.DOM.GetDocument(-1, true);

        // Console.WriteLine(defaultTab.DOM.Document?.RootElement.GetRawText());

        int nodeId = defaultTab.DOM.QuerySelector(1, "input[class=\"wpcf7-form-control wpcf7-submit has-spinner button button--small button--primary-inverse\"]", TimeSpan.FromSeconds(10));
        Console.WriteLine(nodeId);

        Console.WriteLine(defaultTab.DOM.GetBoxModel(nodeId));
    }
}