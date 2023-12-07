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
		// One event handler for all property changes
		public event PropertyChangedEventHandler PropertyChanged;
		public readonly string red = "Red", white = "White";
		private string _statusText;
		// PropertyChanged does not work for array elements
		private string _button0, _button1, _button2, _button3, _button4, _button5;

		public string ButtonColor0
		{
			get
			{
				return _button0;
			}

			set
			{
				if (value == _button0)
					return;

				_button0 = value;

				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ButtonColor0"));
			}
		}

		public string ButtonColor1
		{
			get
			{
				return _button1;
			}

			set
			{
				if (value == _button1)
					return;

				_button1 = value;

				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ButtonColor1"));
			}
		}

		public string ButtonColor2
		{
			get
			{
				return _button2;
			}

			set
			{
				if (value == _button2)
					return;

				_button2 = value;

				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ButtonColor2"));
			}
		}

		public string ButtonColor3
		{
			get
			{
				return _button3;
			}

			set
			{
				if (value == _button3)
					return;

				_button3 = value;

				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ButtonColor3"));
			}
		}

		public string ButtonColor4
		{
			get
			{
				return _button4;
			}

			set
			{
				if (value == _button4)
					return;

				_button4 = value;

				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ButtonColor4"));
			}
		}

		public string ButtonColor5
		{
			get
			{
				return _button5;
			}

			set
			{
				if (value == _button5)
					return;

				_button5 = value;

				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ButtonColor5"));
			}
		}

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

				if (!Intermouse.Initialize(WriteStatus, ColorButton))
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

		public static void ColorButton (ushort index, bool down)
		{
			switch (index) {
				case 0:
					_mainViewModel.ButtonColor0 = down ? _mainViewModel.red : _mainViewModel.white;
					break;
				case 1:
					_mainViewModel.ButtonColor1 = down ? _mainViewModel.red : _mainViewModel.white;
					break;
				case 2:
					_mainViewModel.ButtonColor2 = down ? _mainViewModel.red : _mainViewModel.white;
					break;
				case 3:
					_mainViewModel.ButtonColor3 = down ? _mainViewModel.red : _mainViewModel.white;
					break;
				case 4:
					_mainViewModel.ButtonColor4 = down ? _mainViewModel.red : _mainViewModel.white;
					break;
				case 5:
				default:
					_mainViewModel.ButtonColor5 = down ? _mainViewModel.red : _mainViewModel.white;
					break;
			}
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
//				SHlabel.Content = _mainViewModel.ButtonColor0+_mainViewModel.ButtonColor1+_mainViewModel.ButtonColor2
//					+_mainViewModel.ButtonColor3+_mainViewModel.ButtonColor4+_mainViewModel.ButtonColor5;
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
