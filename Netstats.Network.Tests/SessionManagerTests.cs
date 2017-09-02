using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Netstats.Core.Interfaces;
using Netstats.Network.Api;
using System.Threading.Tasks;

namespace Netstats.Core.Tests
{
    [TestClass]
    public class SessionManagerTests
    {
        ISessionManager sessionManager;

        [TestInitialize]
        public void InitializeNetworkApi()
        {
            sessionManager = new SessionManager(new DummyNetworkApi());
        }

        [TestMethod]
        public void CreateSession()
        {
            var result = sessionManager.CreateSession("qwerty", "foo", CancellationToken.None).Result;
            Assert.AreEqual(result.Id, "19665434");
        }

        [TestMethod]
        public void VisualizeRefreshDataAsync()
        {
            var result = sessionManager.CreateSession("qwerty", "foo", CancellationToken.None).Result;
            var session = result as UserSenseSession;
            var feed = session.GetLatestFeedAsync();
        }
    }
}
