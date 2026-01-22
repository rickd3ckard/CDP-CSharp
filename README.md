# CDP-CSharp ([Visit NuGet package!](https://www.nuget.org/packages/CDP-CSharp/))
Tired of using stupid stacks of cruft for your browser automations? The library you use is out of date and the repo is dead ? Maybe you even tried to dig into the gigabytes of code to implement a fix but the code base is too complex with the 5 layers of abstraction just to open a new tab ? **Vide coding with Claude Sonnet didn't help fix it**?
### Here is the bare minimum we need
I too tried to use those well known libraries to run my company infrastructure. The result ? Complete chaos, no stability and a constant need for monitoring what should be running 24/7 for month with no flaw. This is why CDP-Csharp comes in play. The goal of this repo ? **Build a robust code base with the bare minimum we need to do browser automation**. No external libraries, no ChatGPT including Newtonsoft.JSON, just the raw windows .NET that is well maintained every year. Open a tab ? Simple: connect to the socket and send the command... and guess what ? The tab opens!
### Don't be noob, make it your own 
Oh yeah, but what about my very specific system requirements ? Oh yeah but but, it throws an error message on start up, this is bullsh1t! **Yeah I know, there is bellow 800 lines of executable code**, thus this doesn't cover the whole computer industry. And that's the point. You have access to the tiny code base with no abstraction layer, so open your editor and spend 10 minutes to debug and implement the fix yourself! Can't do that, even with the help of your AI "assistant"? **Consider quitting coding** and find a low IQ job like, I don't know, brick layer.
## Installing the Nuget Package (recommended) 
It is an immense pleasure to inform you that the library is now avaiable as a **[Nuget Package](https://www.nuget.org/packages/CDP-CSharp/)**! Here find the commend for a quick and easy installation: 
### .NET CLI
```bash
dotnet add package Rickd3ckard.CDP-CSharp --version 1.0.0
```
### Package Manager (Visual Studio)
```bash
Install-Package Rickd3ckard.CDP-CSharp -Version 1.0.0
```
## Manual installation guide for kids (dotnet)
This Visual Studio Pro solution comes with **two separate projects**: One class library that contains the core and one console command project for testing and implementing. To get the precious .dll:
```bash
git clone https://github.com/rickd3ckard/CDP-CSharp.git
```
```bash
cd CDP-CSharp
```
```bash
dotnet build "CDP CSharp\CDP CSharp.csproj"
```
Great job, you compiled the library and are ready to use it. I recommand pasting the .dll file inside your GAC_64 folder in order to keep things organised on your computer. **You can then reference this to your new console app project** that will totally not scrape internet ressource for consecutive days - because remember this is not nice thing to do as a good citizen!

*Hint 1: copy C:\Users\*\Desktop\CDP-CSharp\CDP CSharp\bin\Debug\net8.0\CDP CSharp.dll*  
*Hint 2: paste to C:\Windows\Microsoft.NET\assembly\GAC_64\CDP-CSharp*

# Getting started with CDP-Csharp library
Now we got our library referenced inside our new scraping console application... wait no, I mean, learning purpose console application, we can get on going with the coding. First, we can declare a simple `Browser` object - which is very logical thing to do. We simply need to hand the chrome.exe `ChromeExectutablePath`, as well as setting the `Headless` mode as desired. Once done, the `Browser.Start()` command will open the browser. **Oh yeah, and most of the library is async.**
```cs
Browser? browser = await new Browser(@"C:\Program Files (x86)\Google\Chrome\Application\chrome.exe", false).Start();
if (browser == null) { throw new InvalidOperationException(); }
```
Then, once we've got our `Browser` up and running (I recommand Headless as false for developping and true for production), we can grab the default `Tab` object using the 0 index inside the `Browser.Tabs` collection:
```cs
Tab defaultTab = browser.Tabs[0];
```
Any new tab openned will be added to this collection and any closed tab will disapear aswell. The very next logical thing to do is to navigate to a website that we **DO NOT SCRAPE UNDER ANY CIRCUMSTANCES** (MR. GPT SAID) any info from, using the **async** `Tab.NavigateTo()` command:
```cs
await defaultTab.NavigateTo(@"https://web3forms.com/");
```
Great, we are moving in the internet without our hands undetected! We also could with no monitors with `Headless` mode on true. Now we got to the place we intended, we can then query the DOM to get the node tree of the actual webpage. We need to perform this request to be able to navigate the `Tab.DOM` with `DOM.querySelector()` and `DOM.querySelectorAll()` functions. We achieve this with the `Tab.DOM.GetDocument()` function. To open the DOM session, we can just query the first level by setting `Depth = 1` and the `Pierce = true`. If we wanted to grab emails and phones with a regex pattern **for your school research project**, we can query the whole DOM tree in one go by setting the `Depth = -1`.
```cs
await defaultTab.DOM.GetDocument(1, true);
```
