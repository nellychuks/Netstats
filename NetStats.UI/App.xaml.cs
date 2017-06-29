using MahApps.Metro;
using System.Windows;

namespace Netstats.UI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        //Mutex mutex = new Mutex(true, "Netstats");

        protected override void OnStartup(StartupEventArgs e)
        {
            //// get the current app style (theme and accent) from the application
            //// you can then use the current theme and custom accent instead set a new theme
            //Tuple<AppTheme, Accent> appStyle = ThemeManager.DetectAppStyle(Application.Current);

            // now set the Green accent and dark theme
            ThemeManager.ChangeAppStyle(Current,
                                        ThemeManager.GetAccent("Steel"),
                                        ThemeManager.GetAppTheme("BaseLight")); // or appStyle.Item1
                                                                                //Mutext checking 
                                                                                //if (!mutex.WaitOne(TimeSpan.FromSeconds(3)))
                                                                                //    Shutdown();
                                                                                //else
            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            //mutex.ReleaseMutex();
            base.OnExit(e);
        }
    }
}
