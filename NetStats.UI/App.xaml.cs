using MahApps.Metro;
using Netstats.Core;
using Netstats.Core.Interfaces;
using Netstats.Network.Api;
using Splat;
using System.Windows;

namespace Netstats.UI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            //// get the current app style (theme and accent) from the application
            //// you can then use the current theme and custom accent instead set a new theme
            //Tuple<AppTheme, Accent> appStyle = ThemeManager.DetectAppStyle(Application.Current);

            // now set the Green accent and dark theme
            ThemeManager.ChangeAppStyle(Current,
                                        ThemeManager.GetAccent("Steel"),
                                        ThemeManager.GetAppTheme("BaseLight"));
            RegisterServices();
            base.OnStartup(e);
        }

        private void RegisterServices()
        {
            Locator.CurrentMutable.RegisterLazySingleton(() => new SessionManager(new DummyNetworkApi()), typeof(ISessionManager));

            Locator.CurrentMutable.RegisterLazySingleton(() => new UserCredentialsStore(), typeof(UserCredentialsStore));
        }
        protected override void OnExit(ExitEventArgs e)
        {
            //mutex.ReleaseMutex();
            base.OnExit(e);
        }
    }
}
