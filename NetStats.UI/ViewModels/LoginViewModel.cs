using System;
using MahApps.Metro.Controls.Dialogs;
using System.Threading;
using System.Threading.Tasks;
using ReactiveUI;
using System.Reactive.Linq;
using System.Diagnostics;
using Netstats.Core;
using Splat;
using Netstats.Core.Interfaces;

namespace Netstats.UI.ViewModels
{
    public class LoginViewModel : ReactiveObject
    {
        //private Dictionary<SessionCreateFailReaon, string> FailMessages = new Dictionary<SessionCreateFailReaon, string>()
        //{
        //    [SessionCreateFailReaon.AuthenticationFailed] = "Incorrect username or password",
        //    [SessionCreateFailReaon.UnableToReachServer]  = "Check your wireless connection",
        //    [SessionCreateFailReaon.BandwidthExceeded]    = "You have exceeded your monthly quota",
        //    [SessionCreateFailReaon.MaxSessionsReached]   = "You have reached the maximum number of sessions",
        //    [SessionCreateFailReaon.Unknown]              = "An unknown error occurred",
        //};

        private SemaphoreSlim gate = new SemaphoreSlim(1, 1);

        public LoginViewModel(User user)
        {
            var canLogin = this.WhenAnyValue(x => x.Username, x => x.Password,
                (username, password) => !string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password));

            LoginCommand = ReactiveCommand.CreateFromTask(Login);
                       
            GoBackCommand = ReactiveCommand.Create(() => NavigationHelper.NavigateTo(ViewType.BootstrapLoginView, null));

            Username = user != null ? user?.Username : string.Empty;

            Password = user != null ? user?.Password : string.Empty;

            if (user != null)
            {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                Login();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            }
        }

        public ReactiveCommand LoginCommand { get; set; }

        public ReactiveCommand GoBackCommand { get; set; }

        private bool saveuser;
        public bool SaveUser
        {
            get { return saveuser; }
            set { this.RaiseAndSetIfChanged(ref this.saveuser, value); }
        }

        private string username;
        public string Username
        {
            get { return username; }
            set { this.RaiseAndSetIfChanged(ref this.username, value); }
        }

        public string Password
        {
            get { return LoginView.passwordBox.Password; }
            set
            {
                LoginView.passwordBox.Password = value;
                this.RaisePropertyChanged("Password");
            }
        }

        public async Task Login()
        { 
            if (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Password))
            {
                await NavigationHelper.MainWindow.ShowMessageAsync("Opps!", "Please supply a username and password.");
                return; 
            }

            var cancelTokenSource = new CancellationTokenSource();

            var waitDialog = await NavigationHelper.MainWindow.ShowProgressAsync("Sit tight!","Logging in...");

            waitDialog.SetIndeterminate();
            waitDialog.SetCancelable(true);
            waitDialog.Canceled += async (a, b) =>
            {
                cancelTokenSource.Cancel();
                await waitDialog.CloseAsync();
            };
            try
            {
                var session = await Locator.Current.GetService<ISessionManager>().CreateSession(Username, Password, cancelTokenSource.Token);
                await waitDialog.CloseAsync();
                PersistUser();
                Global.CurrentUser = Username;
                NavigationHelper.NavigateTo(ViewType.DashboardView, session);
            }
            // To-do: Add better exception handling
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

        public async void PersistUser()
        {
            var credentialStore = Locator.Current.GetService<UserCredentialsStore>();

            if (!SaveUser || await credentialStore.IsUsernameStored(Username))
                return;

            var result = await NavigationHelper.MainWindow.ShowMessageAsync("Alert", "Specify an alias?", MessageDialogStyle.AffirmativeAndNegative);

            if (result == MessageDialogResult.Affirmative)
            {
                var alias = await NavigationHelper.MainWindow.ShowInputAsync("Info", "Enter your alias");

                if (!await credentialStore.IsUserAliasStored(alias))
                    await credentialStore.AddUser(new User(alias, Username, Password));

                else
                    await NavigationHelper.MainWindow.ShowMessageAsync("Error", "Inavlid alias");
            }

            else
                //Use the username as the alias
                await credentialStore.AddUser(new User(Username, Username, Password));
        }

    }
}
