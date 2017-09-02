//using System.Threading;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using Netstats.Core.Management;
//using Netstats.Core.Management.Interfaces;
//using Netstats.Core.Network.Api;

//namespace Netstats.Core.Tests
//{
//    [TestClass]
//    public class SessionManagerTests
//    {
//        ISessionManager sessionManager;

//        [TestInitialize]
//        public void InitializeNetworkApi()
//        {
//            sessionManager = new SessionManager(new DummyApi());
//        }

//        [TestMethod]
//        public void CreateSession()
//        {
//            var result = sessionManager.CreateSession("qwerty", "foo", CancellationToken.None).Result;
//            Assert.AreEqual(result.Id, "19665434");
//        }
//    }
//}
