namespace Netstats.Network
{
    public struct SessionFeed 
    {
        public double TotalBandwidth { get; set; }
        public BandwidthQuotaType QuotaType { get; set; }
        public double CurrentBandwidth { get { return Upload + Download; } }
        public double Upload { get; set; }
        public double Download { get; set; }
    }
}
