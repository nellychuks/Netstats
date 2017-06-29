using System;
using System.Threading;
using System.Threading.Tasks;

namespace Netstats.Core.Management.Interfaces
{
    public interface ISessionManager
    {
        Task<ISession>CreateSession(string username, string password, CancellationToken token);
        Task<ISession> CreateSession(string username, string password);
        TimeSpan RefreshInterval { get; set; }
        Task DestroyActiveSession();
    }
}
