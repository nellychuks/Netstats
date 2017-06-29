using Netstats.UI.ViewModels;
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

namespace Netstats.UI.Views
{
    /// <summary>
    /// Interaction logic for PinEntryView.xaml
    /// </summary>
    public partial class PinEntryView : UserControl
    {
        public PinEntryView()
        {
            InitializeComponent();
            PinBox = pinTBox;
            pinTBox.PasswordChanged += PinTBox_PasswordChanged;
        }

        public static PasswordBox PinBox { get; internal set; }

        private async void PinTBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (pinTBox.Password.Length == pinTBox.MaxLength)
                await ((PinEntryViewModel)DataContext).UnlockCommand.ExecuteAsyncTask();
        }
    }
}
