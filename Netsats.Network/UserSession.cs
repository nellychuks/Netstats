using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace Netstats.Network.Api
{ 
    public class UserSession
    {
        Subject<Unit> loggedOut = new Subject<Unit>();
        INetworkApi NetworkApi;
        
        public UserSession(string sessionId, INetworkApi networkApi)
        {
            Id = sessionId;
            NetworkApi = networkApi;
            //start refreshing one second after and keep refrshing at equal intervals
            UpdateFeed = Observable.Timer(DateTime.Now + TimeSpan.FromSeconds(1), RefreshInterval)
                                   .Select(_ => GetLatestFeed())
                                   .Catch<SessionFeed, Exception>(e =>
                                   {
                                       loggedOut.OnNext(Unit.Default);
                                       return Observable.Return(new SessionFeed());
                                   });
        }

        public string Id { get; }
        public IObservable<Unit> LoggedOut { get { return loggedOut.AsObservable(); } }
        public IObservable<SessionFeed> UpdateFeed { get; }
        public TimeSpan RefreshInterval { get; set; }

        public SessionFeed GetLatestFeed()
        {
            var response = NetworkApi.Refresh(Id).Result;
            var page = Utilities.IdentifyPage(response);
            if (page == PageKind.SessionPage)
                return Utilities.ParseSessionFeed(response);
            else
                //this would be weird and should probably never happen
                throw new Exception("Unable to refresh feed");
        }

        public async Task Logout()
        {
            var response = await NetworkApi.Logout(Id);
            loggedOut.OnNext(Unit.Default);
        }
    }
}
