
# CDP-CSharp 
Tired of using stupid stacks of cruft for your browser automations? The library you use is out of date and the repo is dead ? Maybe you even tried to dig into the gigabytes of code to implement a fix but the code base is too complex with the 5 layers of abstraction just to open a new tab ? **Vide coding with Claude Sonnet didn't help fix it**?
### Here is the bare minimum we need
I too tried to use those well known libraries to run my company infrastructure. The result ? Complete chaos, no stability and a constant need for monitoring what should be running 24/7 for month with no flaw. This is why CDP-Csharp comes in play. The goal of this repo ? **Build a robust code base with the bare minimum we need to do browser automation**. No external libraries, no ChatGPT including Newtonsoft.JSON, just the raw windows .NET that is well maintained every year. Open a tab ? Simple: connect to the socket and send the command... and guess what ? The tab opens!
### Don't be noob, make it your own 
Oh yeah, but what about my very specific system requirements ? Oh yeah but but, it throws an error message on start up, this is bullsh1t! **Yeah I know, there is bellow 800 lines of executable code**, thus this doesn't cover the whole computer industry. And that's the point. You have access to the tiny code base with no abstraction layer, so open your editor and spen 10 minutes to debug and implement the fix yourself! Can't do that, even with the help of your AI "assistant"? **Consider quitting coding** and find a low IQ job like, I don't know, brick layer.
## Installation guide for kids (dotnet)
This Visual Studio Pro solution comes with **two separate projects**: One class library that contains the core and one console command project for testing and implementing.
```bash
git clone https://github.com/rickd3ckard/CDP-CSharp.git
```
```bash
cd CDP-CSharp
```
```bash
dotnet build "CDP CSharp\CDP CSharp.csproj"
```
Great job, you compiled the library and are ready to use it. I recommand pasting the .dll file inside your GAC_64 folder in order to keep things organised on your computer. **You can then reference this to your new console app project** that will totally not scrape internet ressource doe days - because remember this is not nice thing to do as a good citizen!

*Hint 1: copy C:\Users\*\Desktop\CDP-CSharp\CDP CSharp\bin\Debug\net8.0\CDP CSharp.dll*  
*Hint 2: paste to C:\Windows\Microsoft.NET\assembly\GAC_64\CDP-CSharp*
