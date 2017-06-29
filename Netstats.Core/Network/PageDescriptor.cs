using System;
using System.Linq;
using AngleSharp.Dom.Html;

namespace Netstats.Core.Network
{
    public class PageDescriptor
    {
        public PageKind Identify(IHtmlDocument pageContent)
        {
            PageKind kind = PageKind.UnknownPage;

            /*identity tag for the session pagae*/
            if (pageContent.QuerySelectorAll("p.para1a").Any(elem => elem.TextContent == "Note : If your browser is inactive for more than 23 Hours, then you will be logged out."))
                kind = PageKind.SessionPage;

            /*identity tag for the authentication failed page*/
            else if (pageContent.QuerySelectorAll("p").Any(elem => elem.TextContent == "Authentication Failed"))
                kind = PageKind.AuthenticationFailedPage;

            /*identity tag for the confirmation page*/
            else if (pageContent.QuerySelectorAll("form").Any(x => x.Attributes.Any(att => att.Name == "name" && att.Value == "confirmaction")))
                kind = PageKind.ConfirmationPage;

            /*identity tag for the loggedout page*/
            else if (pageContent.QuerySelectorAll("p").Any(elem => elem.TextContent == "You have been successfully Logged Out!!!"))
                kind = PageKind.LoggedOutPage;

            /*identity tag for the loggedout page*/
            else if (pageContent.QuerySelectorAll("p").Any(elem => elem.TextContent.Contains("Your Bandwidth quota is over")))
                kind = PageKind.BandwidthExceededPage;

            /*identity tag for the loggedout page*/
            else if (pageContent.QuerySelectorAll("p").Any(elem => elem.TextContent.Contains("The no of UserSense session of User:")))
                kind = PageKind.MaxUserSessionsReached;

            return kind;
        }

        private static Lazy<PageDescriptor> instance = new Lazy<PageDescriptor>();
        public static PageDescriptor Instance { get { return instance.Value; } }
    }
}
