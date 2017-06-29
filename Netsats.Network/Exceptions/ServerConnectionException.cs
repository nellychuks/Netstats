using System;

namespace Netstats.Network.Exceptions
{
    public class ServerConnectionFailedException : Exception
    {
        public ServerConnectionFailedException(): base("Unable to connect to remote server")
        {

        }
    }
}
