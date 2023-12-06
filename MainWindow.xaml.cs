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
					MessageBox.Show("Invalid or missing DLL;  closing", "InputInterceptor.Initialize()");
					state = 99;
					Close();
				}

				Intermouse = new Intercept();

				if (!Intermouse.Initialize(WriteStatus))
				{
					MessageBox.Show("No interception", "Intermouse.Initialize()");
					state = 99;
					Close();
				}

			} catch(Exception exception) {
				MessageBox.Show("probably bad: '" 
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
			else if (1 != state)
			{
				SHlabel.Content = $"mouse {Intercept.Stroke[0]} selected";
				Intercept.Selected = Intercept.Stroke[0];

                select.Content = "Click to deselect";
				capture.Visibility = Visibility.Visible;
				state = 1;
			}
			else
			{
				Intercept.Selected = 0;
				capture.Visibility = Visibility.Hidden;
				if (1 < Intercept.devices.Count) {
                	SHlabel.Content = "Left-click 'Select' using mouse to be captured for SimHub";
					select.Content = "Select current device";
					state = 0;
				}
			}
		}

		bool Hooked()		// what to do when a mouse is hooked; e.g. change callback
		{
			Intermouse?.Devices();	// iterate thru intercepted devices
			this.Close();
			return true;
		}

		private void Capture_Click(object sender, RoutedEventArgs e)
		{
			capture.Visibility = Visibility.Hidden;
			Hooked();
		}

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if (99 != state)
			{
				// Handle interception unhooking
				var result = MessageBox.Show("state: " + state
						   + ((0 < state) ? $"; Unhook mouse {Intercept.Selected}?" : "; Done?"),
											 "Closing",			   // messageBox caption
											 MessageBoxButton.YesNo);

				// User doesn't want to close, cancel closure
				e.Cancel = (result == MessageBoxResult.No);
			}
		}

		protected override void OnClosed(EventArgs e)   // called when e.Cancel != true;
		{
			Intermouse?.End();	  	// mouseHook clean up
			base.OnClosed(e);
		}
	}
}
