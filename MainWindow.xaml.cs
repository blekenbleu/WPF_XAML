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
        public MainWindow()
        {
            InitializeComponent();
        }
        static ushort state = 0;

        bool Intercept() {
            this.Close();   
            return true;
        }

        private void select_Click(object sender, RoutedEventArgs e)
        {
            if (state == 0)
            {
                SHlabel.Content = "mouse 12 selected";
                select.Content = "Click to deselect";
                capture.Visibility = Visibility.Visible;
                state = 1;
            }
            else if (state == 1)
            {
                SHlabel.Content = "Left-click 'Select' using mouse to be captured for SimHub";
                select.Content = "Select current device";
                capture.Visibility = Visibility.Hidden;  
                state = 0;
            }
        }

        private void capture_Click(object sender, RoutedEventArgs e)
        {
            Intercept();
        }
    }
}
