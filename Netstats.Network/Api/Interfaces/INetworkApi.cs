using System.Threading;
using System.Threading.Tasks;

namespace Netstats.Network.Api.Interfaces
{
    /// <summary>
    /// General interface for delegating requests ot the server
    /// </summary>
    /// <remarks>
    /// Each method call returns JSON with the appropraite data
    /// </remarks>
    public interface INetworkApi
    {
        Task<string> Login(string username, string password, CancellationToken token);
        Task<string> ConfirmLogin(string username, string password, CancellationToken token, string delete = "yes");
        Task<string> Refresh(string sessionId);  
        Task<string> Logout(string sessionId);
    } 
}