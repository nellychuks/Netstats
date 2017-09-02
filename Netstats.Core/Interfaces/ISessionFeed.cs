namespace Netstats.Core
{
    public interface ISessionFeed
    {
        string Download { get; set; }
        BandwidthQuotaType QuotaType { get; set; }
        string Total { get; set; }
        string Upload { get; set; }
        string Used { get; set; }
    }
}