using Netstats.UI.ViewModels;
using System.Windows.Controls;

namespace Netstats.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class LoginView : UserControl
    {
        public static PasswordBox passwordBox;
        public LoginView()
        {
            var viewModel = DataContext as LoginViewModel;
            InitializeComponent();
            passwordBox = passwordTbox;
        }
    }
}