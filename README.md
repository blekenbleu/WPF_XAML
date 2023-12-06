# WPF_XAML
 Learning exercise for [SimHub mouse interception plugin UI](https://blekenbleu.github.io/static/SimHub/VisualStudio.htm#XAML)  

This is the **intercept** branch for integrating mouse interception.

<details><summary><code>main</code> branch content</summary>

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
	- unlike [WinForm](https://github.com/blekenbleu/WinForm), double-clicking dragged Controls did not generate C# code  
		.. at least not at first, but did when tried later..?!!  
- Launching the app shows prompt in label, current device event in textbox  
		- left button armed to select mouse device  
		- pressing left button reveals *second button* to `capture selected mouse`  
		- left button gets changed to `deselect`  
		- pressing right button, when visible, changes mouse callback to filter  
		- Intercept() may eventually [close the app](https://stackoverflow.com/questions/2820357/how-do-i-exit-a-wpf-application-programmatically).

</details>

<details><summary><a href=https://github.com/oblitum/Interception/releases/latest><b>Interception driver</b> installation</summary>

Keyboard / mouse stroke interception depends on a [**custom signed driver**](https://github.com/oblitum/Interception/releases/latest).
- With *good* luck, [InterceptMouse](https://github.com/blekenbleu/InterceptMouse) automatically installs it.
- Otherwise, reboot the PC and run a Windows Command prompt *as administrator*:
    **InputIntercept\InputInterceptor\Resources>**`install-interception.exe /install`
```
    Interception command line installation tool
    Copyright (C) 2008-2018 Francisco Lopes da Silva

    Interception successfully installed. You must reboot for it to take effect.
```
- then **reboot the PC** before proceeding

#### to uninstall the driver
- I needed to do this for error handling code testing...  
    **InputIntercept\InputInterceptor\Resources>**`install-interception.exe /uninstall`
    - then reboot

</details>

### mouse interception in WPF XAML

- Explicitly [handle <code>OnClosed</code> event](https://learn.microsoft.com/en-us/dotnet/api/system.windows.forms.form.onclosed?view=windowsdesktop-8.0)
	 e.g. to unhook interception.  
	This Closed event follows OnClosing when it is not cancelled.
- Create <code>Intercept.cs</code> class mostly from
	<a href="https://github.com/blekenbleu/InterceptMouse">InterceptMouse/</a><code>Intercept.cs</code>.
- Manually inserted into <code>WPF_XAML.csproj</code>:
```
	<Reference Include="InputIntercept">
		<HintPath>..\InputIntercept\InputInterceptor\bin\Debug\netstandard2.0\InputIntercept.dll</HintPath>
	</Reference>
```

- static class `InputInterceptor` does not get its `Initialize()` invoked automagically (no `New`);  
 	Must invoke `InputInterceptor.Initialize()` to link DLL before instancing `Intercept` class;  
    [InterceptMouse](https://github.com/blekenbleu/InterceptMouse) now behaves the same...  

- Found [a solution for updating XAML TextBox Text from a static method](https://stackoverflow.com/questions/13121155/)

- to do:  
		- detect and exit if no more than one mouse &emsp; &emsp; &emsp; &emsp; &emsp; *done 4 Dec*    
		- access `short[5] Stroke` from callback for mouse selection &nbsp; *done 4 Dec*  
		- filter only selected mouse &emsp; &emsp; &emsp; *done 6 Dec* 
		- implement `short[5]` cumulative displacements instead of increments  &nbsp; *done 6 Dec*  
		- offer to recenter cumulative displacements

- Option to [add console output to WPF app](https://learn.microsoft.com/en-us/answers/questions/168547/project-output-type-forced-to-windows-application):
```
	<DisableWinExeOutputInference>true</DisableWinExeOutputInference>
	<OutputType>Console Application</OutputType> instead of <OutputType>WinExe</OutputType>   
```
