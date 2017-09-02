using System;

namespace Netstats.Core
{
    public class UserSenseSessionFeed :ISessionFeed,  IEquatable<UserSenseSessionFeed>
    {
        public string Total { get; set; } = "0.0 MB";

        public string Used { get; set; } = "0.0 MB";

        public string Download { get; set; } = "0.0 MB";

        public string Upload { get; set; } = "0.0 MB";

        public BandwidthQuotaType QuotaType { get; set; } = BandwidthQuotaType.Unknown;

        
        public bool Equals(UserSenseSessionFeed other)
        {
            return Download == other.Download
                    && Upload == other.Upload && QuotaType == other.QuotaType
                    && Total == other.Total
                    && Used == other.Used;
        }
    }
}
