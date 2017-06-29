using System;

namespace Netstats.Network.Exceptions
{
    public class ServerConnectionFailedException : Exception
    {
        private Exception ex;

        public ServerConnectionFailedException(): base("Unable to connect to remote server")
        {

        }

        public ServerConnectionFailedException(Exception ex)
        {
            this.ex = ex;
        }
    }
}
