using Netstats.Core.Management.Interfaces;

namespace Netstats.Core.Management
{
    public class UserSenseSessionFeed :ISessionFeed
    {
        /* Page-centric properties*/
        public string Download { get; set; }
        public string Upload { get; set; }
        public BandwidthQuotaType QuotaType { get; set; }
        public string TotalBandwidth { get; set; }
        public string UsedBandwidth { get; set; }

        public UserSenseSessionFeed()
        {
            Download = "0.0 MB";
            Upload = "0.0 MB";
            TotalBandwidth = "0.0 MB";
            UsedBandwidth = "0.0 MB";
            QuotaType = BandwidthQuotaType.Unknown;
        }

        public bool Equals(ISessionFeed other)
        {
            return Download == other.Download
                    && Upload == other.Upload && QuotaType == other.QuotaType
                    && TotalBandwidth == other.TotalBandwidth
                    && UsedBandwidth == other.UsedBandwidth;
        }
    }
}
