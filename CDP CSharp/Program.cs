/* 
 * Public domain software, no restrictions. 
 * Released by rickd3ckard: https://github.com/rickd3ckard 
 * See: https://unlicense.org/
 */

using System.Text.Json;
using CDP.Objects;

class Program // test environement
{
    static async Task Main()
    {
        Browser? browser = await new Browser().Start();
        if (browser == null) { return; }

        Tab defaultTab = browser.Tabs[0];

        defaultTab.NavigateTo(@"https://www.maisonsmoches.be/contact/");

        defaultTab.GetDOMDocument(-1,false);

        defaultTab.NavigateTo(@"https://www.youtube.com/watch?v=7wxDn56hF8Y");
        defaultTab.NavigateTo(@"https://www.canva.com/design/DAGwio5xcFM/Z7w7nbe_syCGXkRgiYLsmw/edit");
    }
}