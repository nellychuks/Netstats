using System;
using System.Linq;
using System.Reactive.Linq;

namespace Netstats.Network.Services
{
    public class RefreshService : IRefreshService
    {
        INetworkApi NetworkApi { get; }
        int SessionId { get; }
        public TimeSpan RefreshInterval { get; }
        public IObservable<SessionFeed> UpdateFeed { get; }

        public RefreshService(int sessionId)
        {
            NetworkApi = new UserSenseApi();
            SessionId  = sessionId;
            UpdateFeed = Observable.Interval(RefreshInterval).Select(_ => NetworkApi.Refresh(SessionId.ToString()).Result)
                                                             .Select(res => Utilities.ParseSessionFeed(res));
        }
    }
}
