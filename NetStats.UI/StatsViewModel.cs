using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using NetStats.Core;
using PropertyChanged;
using System.Reactive.Linq;

namespace NetStats.UI
{
    public class StatsViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<DoughnutPoint> UsageChartPoints { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public UserSenseSession Session { get; }

        [AlsoNotifyFor("Upload", "Download")]
        public RefreshFeed Currentfeed { get; set; }

        public string Upload { get { return $"{Currentfeed.Upload}Mb"; } }

        public string Download { get { return $"{Currentfeed.Download}Mb"; } }

        public string QuotaType { get { return Session.QuotaType.ToString(); } }

        IDisposable refreshDisposable;


        public StatsViewModel(UserSenseSession session)
        {
            Session = session;
            refreshDisposable = session.UpdateFeed.SubscribeOnDispatcher()
            .ObserveOnDispatcher()
            .Subscribe(x =>
            {
                Currentfeed = x;
                UsageChartPoints.Clear();
                PopulatePoints(session.TotalBandwidth, Currentfeed.CurrentBandwidth);
            });

            PopulatePoints(session.TotalBandwidth, Currentfeed.CurrentBandwidth);

        }
        private void PopulatePoints(double totalBandwidth, double currentBandwidth)
        {
            UsageChartPoints = new ObservableCollection<DoughnutPoint>()
            {
                new DoughnutPoint(){Category = "Total Bandwidth",Value = totalBandwidth},
                new DoughnutPoint(){Category = "Current Bandwidth",Value = currentBandwidth}
            };
        }
        public void Update([CallerMemberName] string caller = null) => PropertyChanged(this, new PropertyChangedEventArgs(caller));

        public async void Logout()
        {
            refreshDisposable.Dispose();
            await Session.Logout();
        }
    }
}

