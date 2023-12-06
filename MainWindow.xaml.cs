using InputInterceptorNS;
using System;
using System.Windows;
using System.ComponentModel;

namespace WPF_XAML
{
	/// <summary>
	/// https://stackoverflow.com/questions/13121155
	/// a view model class with string property StatusText
	/// </summary>
	public class MainViewModel : INotifyPropertyChanged
	{ 
		private string _statusText;

		public event PropertyChangedEventHandler PropertyChanged;

		public string StatusText
		{
			get
			{
				return _statusText;
			}

			set
			{
				if (value == _statusText)
					return;

				_statusText = value;

				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("StatusText"));
			}
		}
	}

	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		static ushort state = 0;
		Intercept Intermouse;

		// need to reference XAML control from a static method
		private static MainViewModel _mainViewModel = new();

		public MainWindow()
		{
			this.DataContext = _mainViewModel;	// hijack DataContext

			InitializeComponent();

			try
			{
				// InputInterceptor.Initialize() absolutely must be run before new Intercept()!!
				if (!InputInterceptor.Initialize())  // fails if DLL not linked
				{
					MessageBox.Show(Application.Current.MainWindow, "Invalid or missing DLL;  closing", "InputInterceptor.Initialize()");
					state = 99;
					Close();
				}

				Intermouse = new Intercept();

				if (!Intermouse.Initialize(WriteStatus))
				{
					MessageBox.Show(Application.Current.MainWindow, "No interception", "Intermouse.Initialize()");
					state = 99;
					Close();
				}

			} catch(Exception exception) {
				MessageBox.Show(Application.Current.MainWindow, "probably bad: '" 
				+ InputInterceptor.DPath + "'\n" + exception,
				"InputInterceptor.Initialize() Exception"); 
				Close();
			}
		}

		// https://stackoverflow.com/questions/13121155
		public static void WriteStatus(string text)
		{
			_mainViewModel.StatusText = text;		// _mainViewModel is static
		}

		private void Select_Click(object sender, RoutedEventArgs e)
		{
			if (99 == state)
				Hooked();

			if (2 > Intercept.devices.Count)
			{
				SHlabel.Content = "Only one mouse;  none available to capture";
				select.Content = "OK";
				state = 99;
			}
			else if (0 == state)
			{
				SHlabel.Content = $"state {state}:  mouse {Intercept.Stroke[0]} selected";
				Intercept.Selected = Intercept.Stroke[0];

                select.Content = "Click to deselect";
				capture.Visibility = Visibility.Visible;
				state = 1;
			}
			else	// deselect if state 1 or 2
			{
				Intercept.Selected = 0;
				capture.Visibility = Visibility.Hidden;
				if (1 < Intercept.devices.Count) {
                	SHlabel.Content = $"state {state}:  Left-click 'Select' using mouse to be captured for SimHub";
					select.Content = "Select current device";
					capture.Content = "Capture selected device for SimHub use";
					state = 0;
				}
			}
		}

		bool Hooked()		// what to do when a mouse is hooked; e.g. change callback
		{
			Intermouse?.Devices(Intercept.Selected);	// if 0, iterate thru all predicate devices
			this.Close();
			return true;
		}

		private void Capture_Click(object sender, RoutedEventArgs e)
		{
			if (2 == state)
			{
				Intercept.Stroke[1] = Intercept.Stroke[2] = Intercept.Stroke[3] = Intercept.Stroke[4] = 0;
				return;
			}
			// capture.Visibility = Visibility.Hidden;
			capture.Content = "click to center captured coordinates";
			Hooked();
		}

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if (99 != state)
			{
				// Handle interception unhooking
				var result = MessageBox.Show(Application.Current.MainWindow, //"state: " + state + ";  " +
											 ((0 < state) ? $"Unhook mouse {Intercept.Selected} and exit?" : "Done?"),
											 "Closing",			   // messageBox caption
											 MessageBoxButton.YesNo);

				// User doesn't want to close, cancel closure
				e.Cancel = (result == MessageBoxResult.No);
				if (1 == state)
					state++;	// different capture handling
			}
		}

		protected override void OnClosed(EventArgs e)   // called when e.Cancel != true;
		{
			Intermouse?.End();	  	// mouseHook clean up
			base.OnClosed(e);
		}
	}
}
