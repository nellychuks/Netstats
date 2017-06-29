using System;
using Netstats.Core.Management;

namespace Netstats.Core.Api.Exceptions
{
    public class LoginFailedException : Exception
    { 
        public SessionCreateFailReaon FailReason { get; }
        static string errorMsg = "Unable to login";

        public LoginFailedException() : base(errorMsg)
        {
        }

        public LoginFailedException(SessionCreateFailReaon failreason) : base(errorMsg)
        {
            FailReason = failreason;
        }

        public LoginFailedException(SessionCreateFailReaon failreason, Exception ex) : base(errorMsg, ex) 
        {
            FailReason = failreason;
        }
    }
}