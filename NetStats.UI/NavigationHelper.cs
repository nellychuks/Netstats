using Netstats.Core.Management;
using Netstats.Core.Management.Interfaces;
using Netstats.UI.ViewModels;
using Netstats.UI.Views;
using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace Netstats.UI
{
    public enum ViewType
    {
        BootstrapLoginView,

        LoginView,

        DashboardView,

        PinEntryView
    }

    public static class NavigationHelper
    {
        public static Dictionary<ViewType, UserControl> Views = new Dictionary<ViewType, UserControl>()
        {
            [ViewType.BootstrapLoginView] = new BootstrapLoginView(),
            [ViewType.LoginView] = new LoginView(),
            [ViewType.DashboardView] = new DashboardView(),
            [ViewType.PinEntryView] = new PinEntryView()
        };

        public static MainWindow MainWindow { get; set; }
        public static ViewType CurrentView  { get; set; }
        public static ViewType PreviousView { get; set; }

        public static void Initialize(MainWindow mainWindow)
        {
            MainWindow = mainWindow;
        }

        public static void NavigateTo(ViewType view, object args)
        {
            switch (view)
            {
                case ViewType.BootstrapLoginView:
                    Views[ViewType.BootstrapLoginView].DataContext = new BootstrapLoginViewModel();
                    MainWindow.Transitioning.Content = Views[ViewType.BootstrapLoginView];
                    break;

                case ViewType.LoginView:
                    Views[ViewType.LoginView].DataContext = new LoginViewModel(args as User);
                    MainWindow.Transitioning.Content = Views[ViewType.LoginView];
                    break;

                case ViewType.DashboardView:
                    Views[ViewType.DashboardView].DataContext = new DashboardViewModel(args as ISession);
                    MainWindow.Transitioning.Content = Views[ViewType.DashboardView];
                    break;

                case ViewType.PinEntryView:
                    Views[ViewType.PinEntryView].DataContext = new PinEntryViewModel();
                    MainWindow.Transitioning.Content = Views[ViewType.PinEntryView];
                    break;

                default:
                    throw new InvalidOperationException("The specified view is not avaialable");
            }
            PreviousView = CurrentView;
            CurrentView = view;
        }

        public static void NavigateToPrevious()
        {
            MainWindow.Transitioning.Content = Views[PreviousView];
            var temp = PreviousView;
            PreviousView = CurrentView;
            CurrentView = temp;
        }

        public static UserControl GetCurrentView() => Views[CurrentView];
    }
}
 