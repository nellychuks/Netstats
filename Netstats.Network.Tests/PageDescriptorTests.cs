using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AngleSharp.Dom.Html;
using AngleSharp.Parser.Html;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Netstats.Core;
using Netstats.Core.Api;
using Netstats.Core.Network;

namespace Netstats.Network.Tests
{
    [TestClass]
    public class PageDescriptorTests
    {
        static IHtmlDocument sessionPage { get; set; }
        static IHtmlDocument confirmationPage { get; set; }
        static IHtmlDocument AuthenticationFailedPage { get; set; }
        static IHtmlDocument LoggedoutPage { get; set; }
        static Lazy<HtmlParser> singletonParser { get; } = new Lazy<HtmlParser>();
        static HtmlParser SingletonParser { get { return singletonParser.Value; } }

        [TestInitialize]
        public void Initialise()
        {
            sessionPage      = SingletonParser.Parse(File.ReadAllText(@"sessionPage.html"));
            confirmationPage = SingletonParser.Parse(File.ReadAllText(@"confirmationPage.html"));
            AuthenticationFailedPage = SingletonParser.Parse(File.ReadAllText(@"authFailedPage.html"));
            LoggedoutPage    = SingletonParser.Parse(File.ReadAllText(@"logoutPage.html"));
        }

        [TestMethod]
        public void DetectSessionPage()
        {
            Assert.AreEqual(PageKind.SessionPage, PageDescriptor.Instance.Identify(sessionPage));
        }

        [TestMethod]
        public void DetectLoggedoutPage()
        {
            Assert.AreEqual(PageKind.LoggedOutPage, PageDescriptor.Instance.Identify(LoggedoutPage));
        }

        [TestMethod]
        public void DetectConfirmationPage()
        {
            Assert.AreEqual(PageKind.ConfirmationPage, PageDescriptor.Instance.Identify(confirmationPage));
        }

        [TestMethod]
        public void DetectAuthenticationFailedPage()
        {
            Assert.AreEqual(PageKind.AuthenticationFailedPage, PageDescriptor.Instance.Identify(AuthenticationFailedPage));
        }

    }
}
