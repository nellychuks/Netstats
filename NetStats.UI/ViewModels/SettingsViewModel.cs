using Netstats.Core;
using ReactiveUI;
using System;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using MahApps.Metro.Controls.Dialogs;
using ReactiveUI.Legacy;

namespace Netstats.UI.ViewModels
{
    public class SettingsViewModel : ReactiveObject
    {
        SemaphoreSlim gate = new SemaphoreSlim(1, 1);
        public ReactiveCommand<object> DeleteAccountCommand { get; }
        public ReactiveCommand<object> ChangeAccountAliasCommand { get; }
        public ReactiveCommand<object> ChangeAppPinCommand { get; }

        public SettingsViewModel()
        {
            DeleteAccountCommand = ReactiveUI.Legacy.ReactiveCommand.Create();
            DeleteAccountCommand.Subscribe(async _ => await DeleteAccount());

            ChangeAccountAliasCommand = ReactiveUI.Legacy.ReactiveCommand.Create();
            ChangeAccountAliasCommand.Subscribe(async _ => await ChangeAccountAlias());

            ChangeAppPinCommand = ReactiveUI.Legacy.ReactiveCommand.Create();
            ChangeAppPinCommand.Subscribe(async _ => await ChangeAppPin());
        }

        public async Task DeleteAccount()
        {
            var Users = await UserCredentialsStore.Instance.GetUsers();
            var user = Users.FirstOrDefault(x => x.Username == Global.CurrentUser);

            var response = await NavigationHelper.MainWindow.ShowMessageAsync("Alert", $"Are you sure you want to remove {user.Username}?", MessageDialogStyle.AffirmativeAndNegative);
            if (response == MessageDialogResult.Affirmative)
            {
                try
                {
                    await UserCredentialsStore.Instance.DeleteUser(user);
                }
                catch (Exception) { /*This generally should never happen*/ }
                finally { await NavigationHelper.MainWindow.ShowMessageAsync("Alert", $"{user.Username} removed successfully!"); }
            }
        }

        public async Task ChangeAccountAlias()
        {
            var Users = await UserCredentialsStore.Instance.GetUsers();
            var user = Users.FirstOrDefault(x => x.Username == Global.CurrentUser);

            var newAlias = await NavigationHelper.MainWindow.ShowInputAsync("Alert", $"Enter alias");
            if (newAlias == null)
                return;
            if(await UserCredentialsStore.Instance.IsUserAliasStored(newAlias))
            {
                await NavigationHelper.MainWindow.ShowMessageAsync("Error", "Alias is already in use by another account");
                return;
            }
            try
            {
                await UserCredentialsStore.Instance.DeleteUser(user);
                await UserCredentialsStore.Instance.AddUser(new User(newAlias, user.Username, user.Password));
                await NavigationHelper.MainWindow.ShowMessageAsync("Success", $"Successfully changed alias");
            }
            catch (Exception)
            {
                /*This generally should never happen*/
                await NavigationHelper.MainWindow.ShowMessageAsync("Error", "Unable to change alias");
            } 
        }

        private async Task ChangeAppPin()
        {
            var attempt = await NavigationHelper.MainWindow.ShowInputAsync("Alert", "Enter current pin");
            if (attempt == null)
                return;
            if (attempt != (await UserCredentialsStore.Instance.IsEntryPinSet() ? await UserCredentialsStore.Instance.GetEntryPin() : "0000"))
            {
                await NavigationHelper.MainWindow.ShowMessageAsync("Error", "Incorrect pin");
                return;
            }

            var newPin = await NavigationHelper.MainWindow.ShowInputAsync("Alert", "Enter new pin");
            if (newPin.Length == 4)
            {
                await UserCredentialsStore.Instance.SetEntryPin(newPin);
                await NavigationHelper.MainWindow.ShowMessageAsync("Success", "Pin changed!");
            }
        }
    }
}
