using MahApps.Metro.Controls.Dialogs;
using Netstats.Core;
using Netstats.Core.Management;
using Netstats.Core.Management.Interfaces;
using ReactiveUI;
using ReactiveUI.Legacy;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using static Netstats.UI.Utilities;

namespace Netstats.UI
{
    public class DashboardViewModel :ReactiveObject, IDisposable
    {
        IDisposable FeedUpdate { get; }
        public ReactiveCommand<object> LogoutCommand { get; }
        public ReactiveCommand<object> OpenSettingsCommand { get; set; }
        public ObservableCollection<ChartPoint> UsageChartPoints { get; set; }
        public UserSenseSession Session { get; }
        private SemaphoreSlim semaphore { get; } = new SemaphoreSlim(1, 1);
        private bool showSettings;

        public bool ShowSettings
        {
            get { return showSettings; }
            set { this.RaiseAndSetIfChanged(ref showSettings, value); }
        }

        public string TotalBandwidth
        {
            get { return CurrentFeed.TotalBandwidth; }
        }
        public BandwidthQuotaType QuotaType
        {
            get { return CurrentFeed.QuotaType; }
        }
        public string CurrentBandwidth
        {
            get { return CurrentFeed.UsedBandwidth; }
        }
        public string Upload
        {
            get { return CurrentFeed.Upload; }
        }
        public string Download
        {
            get { return CurrentFeed.Download; }
        }
        public string PercentageBandwidth
        {
            get
            {
                return $"{100d -  Math.Round(((ToGigabyte(CurrentBandwidth) / ToGigabyte(TotalBandwidth)) * 100d) == Double.NaN ? 0d : Math.Round(((ToGigabyte(CurrentBandwidth) / ToGigabyte(TotalBandwidth)) * 100f)))}%";
            }
        }

        ISessionFeed currentFeed;
        public ISessionFeed CurrentFeed
        {
            get { return currentFeed ?? new UserSenseSessionFeed(); }
            set
            {
                this.RaiseAndSetIfChanged(ref currentFeed, value);
                this.RaisePropertyChanged("TotalBandwidth");
                this.RaisePropertyChanged("QuotaType");
                this.RaisePropertyChanged("Upload");
                this.RaisePropertyChanged("Download");
                this.RaisePropertyChanged("PercentageBandwidth");
            }
        }

        public DashboardViewModel(ISession session) 
        {
            LogoutCommand = ReactiveUI.Legacy.ReactiveCommand.Create();
            LogoutCommand.Subscribe(async x => await Logout());
            UsageChartPoints = new ObservableCollection<ChartPoint>();
            FeedUpdate = session.RefreshFeed.SubscribeOnDispatcher()
                                            .ObserveOnDispatcher()
                                            //we don't want to update the view with the same data 
                                            //.DistinctUntilChanged(x => x.UsedBandwidth)
                                            .Subscribe(feed => UpdateChart(feed), async _ => await Logout(), () => Debug.WriteLine("Wow!"));
            OpenSettingsCommand = ReactiveUI.Legacy.ReactiveCommand.Create();
            OpenSettingsCommand.Subscribe(_ => NavigationHelper.MainWindow.settingsflyout.IsOpen = true);

        }

        public void Dispose() => FeedUpdate.Dispose();

        public void UpdateChart(ISessionFeed latestFeed)
        {
            CurrentFeed = latestFeed;
            UsageChartPoints.Clear();
            UsageChartPoints.Add(new ChartPoint() { Category = $"Up: {Upload}", Value = ToGigabyte(Upload) });
            UsageChartPoints.Add(new ChartPoint() { Category = $"Down: {Download}", Value = ToGigabyte(Download) });
            UsageChartPoints.Add(new ChartPoint() { Category = $"Left: {3d -ToGigabyte(CurrentBandwidth)} GB", Value = ToGigabyte(TotalBandwidth) - ToGigabyte(CurrentBandwidth) });
        }

        public async Task Logout()
        {
            if (!await semaphore.WaitAsync(50))
                return;
            try
            {
                if (!SessionManager.Instance.HasActiveSession)
                    return;
                Dispose(); 
                await SessionManager.Instance.DestroyActiveSession();
                await NavigationHelper.MainWindow.ShowMessageAsync("Alert", "You have been logged out!");
                Global.CurrentUser = null;
                NavigationHelper.NavigateTo(ViewType.BootstrapLoginView, null);
            }
            finally
            {
                semaphore.Release();
            }
        }
    }

    public class ChartPoint
    {
        public double Value { get; set; }
        public string Category { get; set; }
    }
}

