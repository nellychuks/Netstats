﻿namespace Netstats.Network
{
    //===============================================================================
    // Copyright © Edosa Kelvin.  All rights reserved.
    // THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
    // OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
    // LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
    // FITNESS FOR A PARTICULAR PURPOSE.
    //===============================================================================

    public enum HtmlPageKind
    {
        UnknownPage,

        SessionPage,

        AuthenticationFailedPage,

        LoggedOutPage,

        ConfirmationPage,

        BandwidthExceededPage,

        MaxUserSessionsReached
    }
}
