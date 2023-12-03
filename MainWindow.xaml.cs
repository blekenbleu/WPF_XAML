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

		bool Hooked()		// what to do when a mouse is hooked
		{
			this.Close();
			return true;
		}

		// https://stackoverflow.com/questions/13121155
		public static void WriteStatus(string text)
		{
			_mainViewModel.StatusText = text;		// _mainViewModel is static
		}

		private void Select_Click(object sender, RoutedEventArgs e)
		{
			if (1 != state)
			{
				SHlabel.Content = "mouse 12 selected";
				select.Content = "Click to deselect";
				capture.Visibility = Visibility.Visible;
				state = 1;
			}
			else
			{
				SHlabel.Content = "Left-click 'Select' using mouse to be captured for SimHub";
				select.Content = "Select current device";
				capture.Visibility = Visibility.Hidden;
				state = 0;
			}
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
				var result = MessageBox.Show("state: " + state + ((0 < state) ? "; Unhook mouse?" : "; Done?"),
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
