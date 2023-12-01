using InputInterceptorNS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WPF_XAML
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Intercept Intermouse;

        public MainWindow()
        {
            InitializeComponent();

            if (InputInterceptor.Initialize())
            {   // this seems to make the mouse crazy
                Intermouse = new Intercept();
                Intermouse.Initialize(this);
            }
            else WriteLabel("No interception");
        }
        static ushort state = 0;

        bool Hook()
        {
            this.Close();
            return true;
        }

        public void WriteLabel(string text)
        {
            SHlabel.Content = text;
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
            Hook();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Handle interception unhooking
            var result = MessageBox.Show("state: " + state + ((0 < state) ? "; Unhook mouse?" : "; Done?"),
                                         "Closing",              // messageBox caption
                                         MessageBoxButton.YesNo);

            // User doesn't want to close, cancel closure
            if (result == MessageBoxResult.No)
                e.Cancel = true;
        }

        protected override void OnClosed(EventArgs e)   // called when e.Cancel != true;
        {
            if (null != Intermouse)
                Intermouse.DisposeIntercept();      	// mouseHook clean up
            base.OnClosed(e);
        }
    }
}
