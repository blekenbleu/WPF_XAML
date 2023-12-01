# WPF_XAML
 Learning exercise for [SimHub mouse interception plugin UI](https://blekenbleu.github.io/static/SimHub/VisualStudio.htm#XAML)  

This is the **intercept** branch for integrating mouse interception, currently buggy.

- *Created using GitHub Desktop* `File>New Repository..`  
 ![](NewRepo.png)  
- Then:&nbsp; Visual Studio 2022 Community `File>New`  
 ![](newXAML.png)  
- *VS refused to create a new project in that new repository;*  
 ![](Configure.png)  
- *created new WPF App project in a subfolder, then moved its contents here*  
 ![](MainWindow.png)  

- Dragged WPF Controls (label, textbox and 2 button) from ToolBox into MainWindow  
	- named Controls and added content in `Properties`,  
		adjusted size and margin numbers in `MainWindow.xaml`  
	- Build and Debug:  
		![](SHmouse.png)
	- unlike [WinForm](https://github.com/blekenbleu/WinForm), double-clickng dragged Controls did not generate C# code  
		.. at least not at first, but did when tried later..?!!  
- Launching the app shows prompt in label, current device event in textbox
	- left button armed to select mouse device
		- pressing will unhide second button to capture that selected mouse and change this button to deselect
	- right button, when visible,  will invoke Intercept() when clicked;  
		Intercept() will eventually [close the app](https://stackoverflow.com/questions/2820357/how-do-i-exit-a-wpf-application-programmatically).
- Explicitly [handle <code>Closing</code> event](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/windows/?view=netdesktop-8.0#cancel-window-closure)
	 e.g. to unhook interception
- Create <code>Intercept.cs</code> class mostly from
	<a href="https://github.com/blekenbleu/InterceptMouse">InterceptMouse/</a><code>Program.cs</code>.
- Manually inserted into <code>WPF_XAML.csproj</code>:
<pre>
	&lt;Reference Include="InputInterceptor-PersonalFork"&gt;
		&lt;HintPath&gt;..\InputInterceptor-PersonalFork\InputInterceptor\bin\Debug\netstandard2.0\InputInterceptor-PersonalFork.dll&lt;/HintPath&gt;
	&lt;/Reference&gt;
</pre>
- static class InputInterceptor does not get its Initialize() invoked automagically (no New);  
 	Must invoke InputInterceptor.Initialize() to link DLL before invoking Intercept class;  
    don't know why InterceptMouse does not also crash...  
- Intercept class causes mouse to go crazy..
