// https://github.com/0x2E757/InputInterceptor/ Example Application
using InputInterceptorNS;
using System;
using System.Collections.Generic;
using System.Windows;
using Context = System.IntPtr;
using Device = System.Int32;

namespace WPF_XAML
{
	/// <summary>
	/// Mouse Interception handling for MainWindow.xaml.cs
	/// </summary>
	public class Intercept
	{
		static List<DeviceData> devices;
		static bool once = false;
		static MainWindow main;

		MouseHook Mousehook { get; } = new(MouseCallback);

		public void Initialize(MainWindow passed)
		{
			main = passed;
			devices = null;

			if (true) // InitializeDriver())
			{
				once = true;

				//  KeyboardHook keyboardHook = new KeyboardHook(KeyboardCallback);
				WriteLabel("Mouse Hook enabled.  Left-click 'Select' using mouse to be captured for SimHub");
			}
			else
			{
				InstallDriver();
			}
		}

		static void WriteLabel(string label) { main.WriteLabel(label);  }


		public void DisposeIntercept()
		{
			//  keyboardHook.Dispose();
			if (once)
				Mousehook.Dispose();
		}

		// https://learn.microsoft.com/en-us/dotnet/framework/interop/how-to-implement-callback-functions
		private static bool MouseCallback(Context context, Device device, ref MouseStroke m)
		{
			try
			{
				if (true == once) {
					once = false;
					devices = InputInterceptor.GetDeviceList(context, InputInterceptor.IsMouse);
				}
				string scroll = (0 == (0xC00 & (ushort)m.State)) ? "" : $" x:{XY(ref m, 11)}, y:{XY(ref m, 10)}";
				// Mouse XY coordinates are raw changes
				Report($"Device: {device}; MouseStroke: X:{m.X}, Y:{m.Y}; S: {m.State}" + scroll);
			}
			catch (Exception exception)
			{
				WriteLabel($"MouseStroke: {exception}");
			}

			//  m.X = -m.X;	 // Invert mouse X
			//  m.Y = -m.Y;	 // Invert mouse Y
			return true;
		}

		// decode scrolling
		private static short XY(ref MouseStroke m, short s) { return (short)((((UInt16)m.State >> s) & 1) * ((m.Rolling < 0) ? -1 : 1)); }

		private static void Report (string message) { WriteLabel(message); }

		private static bool KeyboardCallback(Context context, Device device, ref KeyStroke keyStroke)
		{
			try
			{
				WriteLabel($"Device: {device}; Keystroke: {keyStroke.Code} {keyStroke.State} {keyStroke.Information}");
			}
			catch (Exception exception)
			{
				WriteLabel($"KeyStroke: {exception}");
			}
			// Button swap
			//keyStroke.Code = keyStroke.Code switch {
			//  KeyCode.A => KeyCode.B,
			//  KeyCode.B => KeyCode.A,
			//  _ => keyStroke.Code,
			//};
			return true;
		}

		static bool InitializeDriver()
		{
			if (InputInterceptor.CheckDriverInstalled())
			{
				WriteLabel("Input interceptor seems to be installed.");
				if (InputInterceptor.Initialize())
				{
					WriteLabel("Input interceptor successfully initialized.");
					return true;
				}
			}
			WriteLabel("Input interceptor initialization failed.");
			return false;
		}

		static void InstallDriver()
		{
			WriteLabel("Input interception driver not installed.");
			if (InputInterceptor.CheckAdministratorRights())
			{
				WriteLabel("Installing...");
				if (InputInterceptor.InstallDriver())
				{
					WriteLabel("Input interception driver installed! Restart your computer.");
				}
				else
				{
					WriteLabel("Something... gone... wrong... :(");
				}
			}
			else
			{
				WriteLabel("Run InputInterceptori\\Resources\\install-interception.exe to install the required driver.");
			}
		}
	}
}
