using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Netstats.Network
{
    public class LogoutResult : NetworkResult
    {
        public LogoutResult(bool success  = true, NetworkErrorKind error = NetworkErrorKind.None) : base(success, error)
        {

        }
    }
}
