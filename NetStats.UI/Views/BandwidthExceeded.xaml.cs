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

namespace NetStats.UI.Views
{
    /// <summary>
    /// Interaction logic for BandwidthExceeded.xaml
    /// </summary>
    public partial class BandwidthExceeded : UserControl
    {
        private MainWindow window;

        public BandwidthExceeded()
        {
            InitializeComponent();
            window = new MainWindow();
        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            window.transitioning.Content = window;
        }
    }
}
