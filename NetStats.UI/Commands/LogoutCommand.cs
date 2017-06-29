using Netstats.UI;
using System;
using System.Windows.Input;

namespace NetStats.UI.Commands
{
    public class LogoutCommand: ICommand
    {
        public event EventHandler CanExecuteChanged;
        public DashboardViewModel ViewModel { get; }

        public LogoutCommand(DashboardViewModel viewModel)
        {
            ViewModel = viewModel;
        }

        public bool CanExecute(object parameter) => true;

        public async void Execute(object parameter) => await ViewModel.Logout();
    }
}
