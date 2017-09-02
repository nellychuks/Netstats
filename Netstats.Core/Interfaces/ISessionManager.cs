using System;
using System.Threading;
using System.Threading.Tasks;

namespace Netstats.Core.Interfaces
{
    public interface ISessionManager
    {
        ISession CurrentSession { get; }

        Task<ISession>CreateSession(string username, string password, CancellationToken token);

        Task<ISession> CreateSession(string username, string password);

        Task DestroyCurrentSessionAsync();
    }
}
