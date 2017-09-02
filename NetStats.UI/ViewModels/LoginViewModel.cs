using System;
using MahApps.Metro.Controls.Dialogs;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using ReactiveUI;
using System.Reactive.Linq;
using Netstats.Core.Management;
using Netstats.Core.Api.Exceptions;
using System.Diagnostics;
using ReactiveUI.Legacy;

namespace Netstats.UI.ViewModels
{
    public class LoginViewModel : ReactiveObject
    {
        private Dictionary<SessionCreateFailReaon, string> FailMessages = new Dictionary<SessionCreateFailReaon, string>()
        {
            [SessionCreateFailReaon.AuthenticationFailed] = "Incorrect username or password",
            [SessionCreateFailReaon.UnableToReachServer]  = "Check your wireless connection",
            [SessionCreateFailReaon.BandwidthExceeded]    = "You have exceeded your monthly quota",
            [SessionCreateFailReaon.MaxSessionsReached]   = "You have reached the maximum number of sessions",
            [SessionCreateFailReaon.Unknown]              = "An unknown error occurred",
        };
        private SemaphoreSlim gate = new SemaphoreSlim(1, 1);

        public LoginViewModel(User user)
        {
            LoginCommand = ReactiveUI.ReactiveCommand.CreateFromTask(Login);
                       
            GoBackCommand = ReactiveUI.Legacy.ReactiveCommand.Create();
      
            GoBackCommand.Subscribe(x => NavigationHelper.NavigateTo(ViewType.BootstrapLoginView, null));

            Username = user != null ? user?.Username : string.Empty;
            Password = user != null ? user?.Password : string.Empty;

            if (user != null)
            {
                //Start login request automatically
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                Login();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            }
        }

        public ReactiveUI.ReactiveCommand LoginCommand { get; private set; }
        public ReactiveUI.Legacy.ReactiveCommand<object> GoBackCommand { get; private set; }

        public User User { get; }

        private bool saveuser;
        public bool SaveUser { get { return saveuser; } set { this.RaiseAndSetIfChanged(ref this.saveuser, value); } }

        private string username;
        public string Username { get { return username; } set { this.RaiseAndSetIfChanged(ref this.username, value); } }

        public string Password
        {
            get { return LoginView.passwordBox.Password; }
            set
            {
                LoginView.passwordBox.Password = value;
                this.RaisePropertyChanged("Password");
            }
        }

        public bool IsLoggedIn { get; private set; }

        public async void ConsolidateLogin()
        {
            if (!SaveUser || await UserCredentialsStore.Instance.HasUsername(Username))
                return;

            var result = await NavigationHelper.MainWindow.ShowMessageAsync("Alert", "Specify an alias?", MessageDialogStyle.AffirmativeAndNegative);
            if (result == MessageDialogResult.Affirmative)
            {
                var alias = await NavigationHelper.MainWindow.ShowInputAsync("Info", "Enter your alias");
                if (!await UserCredentialsStore.Instance.HasUserAlias(alias))
                    await UserCredentialsStore.Instance.AddUser(new User(alias, Username, Password));
                else
                    await NavigationHelper.MainWindow.ShowMessageAsync("Error", "Inavlid alias");
            }
            else
            {
                //Use the username as thealias
                await UserCredentialsStore.Instance.AddUser(new User(Username, Username, Password));
            }
        }


        public async Task Login()
        { 
            //validate credentials
            if (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Password))
            {
                await NavigationHelper.MainWindow.ShowMessageAsync("Woah!", "Please supply a Username and Password.");
                return; 
            }
            var cancelTokenSource = new CancellationTokenSource();
            var waitDialog = await NavigationHelper.MainWindow.ShowProgressAsync("Logging in...", "Authenticating...");
            waitDialog.SetIndeterminate();
            waitDialog.SetCancelable(true);
            waitDialog.Canceled += async (a, b) =>
            {
                cancelTokenSource.Cancel();
                await waitDialog.CloseAsync();
            };
            try
            {
                var session = await SessionManager.Instance.CreateSession(Username, Password, cancelTokenSource.Token);
                await waitDialog.CloseAsync();
                ConsolidateLogin();
                Global.CurrentUser = Username;
                NavigationHelper.NavigateTo(ViewType.DashboardView, session);
            }
            catch (LoginFailedException ex)
            {
                await waitDialog.CloseAsync();
                await NavigationHelper.MainWindow.ShowMessageAsync("Opps!", $"{FailMessages[(ex as LoginFailedException).FailReason]}", MessageDialogStyle.Affirmative);
            }
            catch (TaskCanceledException)
            {
                await waitDialog.CloseAsync();
            }
            catch (Exception ex)when (ex.Message == "Already processing a request")
            {
              /**/
            }
            catch(Exception ex)
            {
                await waitDialog.CloseAsync();
                await NavigationHelper.MainWindow.ShowMessageAsync("Opps!", "An Unknown error occurred");
                Debug.WriteLine(ex.Message);
            }
        }
    }
}
