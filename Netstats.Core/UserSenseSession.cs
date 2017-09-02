using System;
using System.Linq;
using System.Reactive.Linq;
using Netstats.Core.Interfaces;
using Netstats.Network.Api.Interfaces;
using Newtonsoft.Json;

namespace Netstats.Core
{
    public class UserSenseSession : ISession
    {
        public INetworkApi NetworkApi { get; }

        public UserSenseSession(string id, CoreSettings settings, INetworkApi networkApi)
        {
            Id = id;
            NetworkApi = networkApi;
            /*start refreshing after 1.5s after and keep refrshing at equal intervals*/
            RefreshFeed = Observable.Timer(TimeSpan.FromSeconds(1.5), settings.RefreshInterval)
                                    .Select(_ => GetLatestFeedAsync());
        }

        public string Id { get; }

        public IObservable<ISessionFeed> RefreshFeed { get; }

        public CoreSettings Settings { get; }

        public ISessionFeed GetLatestFeedAsync()
        {
            var json = NetworkApi.Refresh(Id).Result;

            return JsonConvert.DeserializeObject<UserSenseSessionFeed>(json) as ISessionFeed;
        }
    }
}
