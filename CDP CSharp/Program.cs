/* 
 * Public domain software, no restrictions. 
 * Released by rickd3ckard: https://github.com/rickd3ckard 
 * See: https://unlicense.org/
 */

using CDP.Objects;
using System.Text.Json;

class Program // test environement
{
    static async Task Main()
    {
        Browser? browser = await new Browser().Start();
        if (browser == null) { return; }

        Tab defaultTab = browser.Tabs[0];

        defaultTab.NavigateTo(@"https://www.maisonsmoches.be/contact/");
        JsonDocument DOM = defaultTab.DOM.GetDocument(-1, false);
        Console.WriteLine(DOM.RootElement.GetRawText()) ;
    }
}