using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Netstats.Network.Exceptions;
using System.Threading;
using Netstats.Network.Services;

namespace Netstats.Network.Tests
{
    [TestClass]
    public class ApiTests
    {
        ILoginService loginService;

        [TestInitialize]
        public void InitializeNetworkApi()
        {
            loginService = new LoginService();
        }

        [TestMethod]
        public void LoginTest()
        {
            Assert.IsNull(loginService.Login("", "", CancellationToken.None).Result);
        }
    }
}
