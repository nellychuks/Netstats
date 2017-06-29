﻿using System;

namespace Netstats.Core.Api.Exceptions
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
