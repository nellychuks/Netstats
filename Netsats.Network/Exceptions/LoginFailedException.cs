using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Netstats.Network.Exceptions
{
    public class LoginFailedException : Exception
    { 
        public LoginFailReason FailReason { get; }

        public LoginFailedException() : base("Unable to login")
        {
        }

        public LoginFailedException(LoginFailReason failreason) : base("Unable to login")
        {
            FailReason = failreason;
        }
    }
}
