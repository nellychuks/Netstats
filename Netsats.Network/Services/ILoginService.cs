using System.Threading;
using System.Threading.Tasks;

namespace Netstats.Network.Services
{
    public interface ILoginService
    {
        Task<LoginResult> Login(string username, string password, CancellationToken cancelToken);
        Task Logout(string sessionId);
    }
}
