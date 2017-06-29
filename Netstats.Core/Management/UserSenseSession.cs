using System;
using System.Linq;
using System.Reactive.Linq;
using Netstats.Core.Management.Interfaces;
using Netstats.Core.Network;
using Netstats.Core.Network.Api;

namespace Netstats.Core.Management
{
    public class UserSenseSession : ISession
    {
        public INetworkApi NetworkApi { get; }

        public UserSenseSession(string id, INetworkApi networkApi)
        {
            Id = id; 
            NetworkApi = networkApi;
            /*start refreshing three second after and keep refrshing at equal intervals*/
            RefreshFeed = Observable.Timer(DateTime.Now, RefreshInterval)
                                    .Select(_ => GetLatestFeed());
        }

        public string Id { get; }
        public IObservable<ISessionFeed> RefreshFeed { get; }
        public TimeSpan RefreshInterval { get; set; } = TimeSpan.FromSeconds(2);
       
        public ISessionFeed GetLatestFeed()
        {
            var response = PageMixins.ParsePage(NetworkApi.Refresh(Id).Result);
            if (response.Kind != PageKind.SessionPage)
                /*We got the wrong page*/
                throw new Exception("Unable to fetch latest feed");
            return ParseFeed(response);
        }

        public ISessionFeed ParseFeed(IPage page)
        {
            return page.Content == null ?
            new UserSenseSessionFeed()
            :
            new UserSenseSessionFeed
            {
                Download       = PageMixins.GetAllSiblings(page.Content, "td.para1", "Download:")
                                           .Select(x => x.TextContent)
                                           .FirstOrDefault(),
                Upload         = PageMixins.GetAllSiblings(page.Content, "td.para1", "Upload:")
                                           .Select(x => x.TextContent)
                                           .FirstOrDefault(),
                QuotaType      = (BandwidthQuotaType)Enum.Parse(typeof(BandwidthQuotaType), PageMixins.GetAllSiblings(page.Content, "td.para1", "Bandwidth Quota Schedule:")
                                           .Select(x => x.TextContent)
                                           .FirstOrDefault()),
                TotalBandwidth = PageMixins.GetAllSiblings(page.Content, "td.para1", "Group Allowed Bandwidth: ")
                                           .Select(x => x.TextContent)
                                           .FirstOrDefault(),
                UsedBandwidth  = PageMixins.GetAllSiblings(page.Content, "td.para1", "Total Bandwidth:")
                                           .Select(x => x.TextContent)
                                           .FirstOrDefault(),
            };
        }
    }
}
