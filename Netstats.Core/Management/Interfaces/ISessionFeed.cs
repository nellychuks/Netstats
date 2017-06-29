using System;

namespace Netstats.Core.Management.Interfaces
{
    public interface ISessionFeed : IEquatable<ISessionFeed>
    {
        string Download { get; set; }
        BandwidthQuotaType QuotaType { get; set; }
        string TotalBandwidth { get; set; }
        string Upload { get; set; }
        string UsedBandwidth { get; set; }
    }
}