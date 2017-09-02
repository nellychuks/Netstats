using Microsoft.VisualStudio.TestTools.UnitTesting;
using Netstats.Network.Api;
using Netstats.Network.Api.Interfaces;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Netstats.Network.Tests
{
    [TestClass]
    public class ApiTests
    {
        INetworkApi NetworkApi { get; set; }

        [TestInitialize]
        public void InitializeNetworkApi()
        {
            NetworkApi = new DummyNetworkApi();
        }

        [TestMethod]
        public async Task ValidateLoginFeedbackAsync()
        {
            var result = await NetworkApi.Login("qwerty", "uiop", CancellationToken.None);
            Debug.WriteLine(result);
        }

        [TestMethod]
        public async Task ValidateLogoutFeedbackAsync()
        {
            var result = await NetworkApi.Login("qwerty", "uiop", CancellationToken.None);
            Debug.WriteLine(result);
        }

        [TestMethod]
        public async Task ValidateRefreshFeedbackAsync()
        {
            var result = await NetworkApi.Refresh("qwerty");
            Debug.WriteLine(result);
        }
    }
}
