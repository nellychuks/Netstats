using System;
using System.Windows.Input;
using Netstats.UI.ViewModels;

namespace NetStats.UI
{
    public class ProceedToLoginCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;
        public BootstrapLoginViewModel viewModel;

        public ProceedToLoginCommand(BootstrapLoginViewModel viewModel)
        {
            this.viewModel = viewModel;
        }
        public bool CanExecute(object parameter) => true;

        public void Execute(object parameter) => viewModel.ProceedToLogin((parameter as string));
    }
}
