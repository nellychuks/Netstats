using System.ComponentModel;
using MahApps.Metro.Controls;
using System;
using System.Windows.Forms;
using System.Drawing;
using System.Windows;
using MahApps.Metro.Controls.Dialogs;
using Netstats.Core.Management;
using Akavache;

namespace Netstats.UI.Views
{
    /// <summary>
    /// Interaction logic for ShellView.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public static Flyout SettingFlyout;
        private NotifyIcon notifyIcon = new NotifyIcon();
        private ShellViewModel ViewModel;

        public MainWindow()
        {
            InitializeComponent();
            SetupBlobCache();
            SetupTrayIcon();
            DataContext = ViewModel = new ShellViewModel();
            NavigationHelper.Initialize(this);
            NavigationHelper.NavigateTo(ViewType.BootstrapLoginView, null);
            SettingFlyout = settingsflyout;
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            ViewModel.LockApp();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        }

        void SetupBlobCache()
        {
            BlobCache.ApplicationName = "Netstats";
        }

        void SetupTrayIcon()
        {
            notifyIcon.Icon = new Icon("aui_logo.png.ico");
            notifyIcon.DoubleClick += (s,e) => WindowState = WindowState.Normal; 
        }


        protected override async void OnStateChanged(EventArgs e)
        {
            if (WindowState == WindowState.Minimized)
            {
                ShowInTaskbar = false;
                notifyIcon.Visible = true;
                await ViewModel.LockApp();
            }
            else if (WindowState == WindowState.Normal)
            {
                notifyIcon.Visible = false;
                ShowInTaskbar = true;
            }
        }

        protected async override void OnClosing(CancelEventArgs e)
        {
            if (Global.IsAppLocked)
            {
                e.Cancel = true;
                return;
            }

            if (Global.IsLoggedIn)
            {
                e.Cancel = true;
                var response = await this.ShowMessageAsync("Alert", $"Currently logged in as {Global.CurrentUser}. Do you want to logout first?", MessageDialogStyle.AffirmativeAndNegative);
                if (response == MessageDialogResult.Affirmative)
                {
                    await ((DashboardViewModel)NavigationHelper.Views[ViewType.DashboardView].DataContext).Logout();
                    Global.CurrentUser = null;
                    Close();
                }
                else
                    return;
            }

            notifyIcon.Dispose();
            UserCredentialsStore.Instance.ShutDown();
        }

        public void BringToFront()
        {
            if (WindowState == WindowState.Minimized)
                WindowState = WindowState.Normal;
        }
    }
}
