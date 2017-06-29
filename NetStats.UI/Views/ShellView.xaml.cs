using System.ComponentModel;
using MahApps.Metro.Controls;
using Netstats.UI.ViewModels;
using Netstats.DataDomain;
using NetStats.UI.Views;

namespace Netstats.UI.Views
{
    /// <summary>
    /// Interaction logic for ShellView.xaml
    /// </summary>
    public partial class ShellView : MetroWindow
    {
        public TransitioningContentControl Transitioning { get; }

        public ShellView()
        {
            InitializeComponent();
            PersistentStore.SaveUser(new UserCredentials("Kelvin", "JohnPaul"));
            Transitioning = new TransitioningContentControl() { Transition = TransitionType.LeftReplace };
            LayoutRoot.Children.Add(Transitioning);
            Transitioning.Content = new BootstrapLoginView() { DataContext = new BootstrapLoginViewModel(this) };
        }

        protected override void OnClosing(CancelEventArgs e) => PersistentStore.ShutDown();
    }
}
