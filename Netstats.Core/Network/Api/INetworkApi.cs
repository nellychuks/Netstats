using System.Threading;
using System.Threading.Tasks;

namespace Netstats.Core.Network.Api
{
    public interface INetworkApi
    {
        Task<string> Login(string username, string password, CancellationToken token);
        Task<string> ConfirmLogin(string username, string password, CancellationToken token, string delete = "yes");
        Task<string> Refresh(string sessionId);
        Task<string> Logout(string sessionId);
    }
}