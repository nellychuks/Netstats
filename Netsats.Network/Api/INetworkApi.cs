using AngleSharp.Dom.Html;
using System.Threading;
using System.Threading.Tasks;

namespace Netstats.Network
{
    public interface INetworkApi
    {
        Task<IHtmlDocument> Login(string username, string password, CancellationToken token);
        Task<IHtmlDocument> ConfirmLogin(string username, string password, CancellationToken token, string delete = "yes");
        Task<IHtmlDocument> Refresh(string sessionId);
        Task<IHtmlDocument> Logout(string sessionId);
    }
}