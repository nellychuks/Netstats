using System;
using ReactiveUI;
using Netstats.UI;
using MahApps.Metro.Controls.Dialogs;
using Netstats.Core;
using System.Reactive.Linq;
using Netstats.UI.Views;
using System.Threading;
using Netstats.Core.Management;

namespace Netstats.UI.ViewModels
{
    public class PinEntryViewModel : ReactiveObject
    {
        private SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);
        public string Pin
        {
            get { return PinEntryView.PinBox.Password; }
            set { PinEntryView.PinBox.Password = value; this.RaisePropertyChanged("Pin"); }
        }

        public PinEntryViewModel()
        {
            Pin = string.Empty;
            UnlockCommand = ReactiveCommand.Create();
            UnlockCommand.Subscribe(async _ =>
            {
                await semaphore.WaitAsync();
                if (!Global.IsAppLocked)
                    return;
                var pin = await UserCredentialsStore.Instance.IsEntryPinSet() ? await UserCredentialsStore.Instance.GetEntryPin() : "0000";
                if (Pin == pin)
                {
                    Global.IsAppLocked = false;
                    NavigationHelper.NavigateToPrevious();

                    //execute all waiting actions
                    while(Global.OnNextUnlockActions.Count > 0)
                    {
                        Global.OnNextUnlockActions.Pop()();
                    }
                }
                else
                {
                    await NavigationHelper.MainWindow.ShowMessageAsync("Alert", "Incorrect Pin!");
                    Pin = string.Empty;
                }
                semaphore.Release();
             });
        }

        public ReactiveCommand<object> UnlockCommand { get; private set; }
    }
}
