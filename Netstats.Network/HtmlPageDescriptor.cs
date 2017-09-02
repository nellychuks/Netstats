using AngleSharp.Dom.Html;
using System.Linq;

namespace Netstats.Network
{
    //===============================================================================
    // Copyright © Edosa Kelvin.  All rights reserved.
    // THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
    // OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
    // LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
    // FITNESS FOR A PARTICULAR PURPOSE.
    //===============================================================================

    public static class HtmlPageDescriptor
    {
        public static HtmlPageKind Identify(IHtmlDocument content)
        {
            HtmlPageKind kind = HtmlPageKind.UnknownPage;

            /*identity tag for the session pagae*/
            if (content.QuerySelectorAll("p.para1a").Any(elem => elem.TextContent == "Note : If your browser is inactive for more than 23 Hours, then you will be logged out."))
                kind = HtmlPageKind.SessionPage;

            /*identity tag for the authentication failed page*/
            else if (content.QuerySelectorAll("p").Any(elem => elem.TextContent == "Authentication Failed"))
                kind = HtmlPageKind.AuthenticationFailedPage;

            /*identity tag for the confirmation page*/
            else if (content.QuerySelectorAll("form").Any(x => x.Attributes.Any(att => att.Name == "name" && att.Value == "confirmaction")))
                kind = HtmlPageKind.ConfirmationPage;

            /*identity tag for the loggedout page*/
            else if (content.QuerySelectorAll("p").Any(elem => elem.TextContent == "You have been successfully Logged Out!!!"))
                kind = HtmlPageKind.LoggedOutPage;

            /*identity tag for the loggedout page*/
            else if (content.QuerySelectorAll("p").Any(elem => elem.TextContent.Contains("Your Bandwidth quota is over")))
                kind = HtmlPageKind.BandwidthExceededPage;

            /*identity tag for the loggedout page*/
            else if (content.QuerySelectorAll("p").Any(elem => elem.TextContent.Contains("The no of UserSense session of User:")))
                kind = HtmlPageKind.MaxUserSessionsReached;

            return kind;
        }
    }
}
