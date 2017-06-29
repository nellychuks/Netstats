﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Netstats.Network
{
    public enum NetworkErrorKind
    {
        None,

        AuthenticationFailed,

        BandwidthExceeded,

        UnableToMakeReqest,

        UnableToRefresh,

        UnableToConfirm,

        Unkown,
    }
}
