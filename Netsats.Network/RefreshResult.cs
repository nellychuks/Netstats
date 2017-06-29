using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetsatsCoreApi
{
    public class RefreshResult : NetworkResult
    {
        public double TotalBandwidth { get; set; }
        public BandwidthQuotaType QuotaType { get; set; }
        public double CurrentBandwidth { get { return Upload + Download; } }
        public double Upload { get; set; }
        public double Download { get; set; }

        public RefreshResult(bool success = true, NetworkErrorKind error = NetworkErrorKind.None) : base(success, error)
        {

        } 
    }
}
