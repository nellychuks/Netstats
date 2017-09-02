using System.IO;
using AngleSharp.Dom.Html;
using AngleSharp.Parser.Html;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Netstats.Network.Tests
{
    [TestClass]
    public class PageDescriptorTests
    {
        static IHtmlDocument SessionPage { get; set; }
        static IHtmlDocument ConfirmationPage { get; set; }
        static IHtmlDocument AuthenticationFailedPage { get; set; }
        static IHtmlDocument LoggedoutPage { get; set; }
        static HtmlParser SingletonParser { get; set; } = new HtmlParser();

        [TestInitialize]
        public void Initialise()
        {
            SessionPage      = SingletonParser.Parse(File.ReadAllText(@"sessionPage.html"));
            ConfirmationPage = SingletonParser.Parse(File.ReadAllText(@"confirmationPage.html"));
            AuthenticationFailedPage = SingletonParser.Parse(File.ReadAllText(@"authFailedPage.html"));
            LoggedoutPage    = SingletonParser.Parse(File.ReadAllText(@"logoutPage.html"));
        }

        [TestMethod]
        public void DetectSessionPage()
        {
            Assert.AreEqual(HtmlPageKind.SessionPage, HtmlPageDescriptor.Identify(SessionPage));
        }

        [TestMethod]
        public void DetectLoggedoutPage()
        {
            Assert.AreEqual(HtmlPageKind.LoggedOutPage, HtmlPageDescriptor.Identify(LoggedoutPage));
        }

        [TestMethod]
        public void DetectConfirmationPage()
        {
            Assert.AreEqual(HtmlPageKind.ConfirmationPage, HtmlPageDescriptor.Identify(ConfirmationPage));
        }

        [TestMethod]
        public void DetectAuthenticationFailedPage()
        {
            Assert.AreEqual(HtmlPageKind.AuthenticationFailedPage, HtmlPageDescriptor.Identify(AuthenticationFailedPage));
        }

    }
}
