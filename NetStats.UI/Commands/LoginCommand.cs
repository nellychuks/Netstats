using Netstats.UI.ViewModels;
using System;
using System.Windows.Input;

namespace NetStats.UI.Commands
{
    public class LoginCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;
        public LoginViewModel ViewModel { get; }

        public LoginCommand(LoginViewModel viewModel)
        {
            ViewModel = viewModel;
        }

        public bool CanExecute(object parameter) => true;

        public async void Execute(object parameter) => await ViewModel.Login();
      }
}
