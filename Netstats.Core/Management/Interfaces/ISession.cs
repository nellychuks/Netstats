using System;

namespace Netstats.Core.Management.Interfaces
{
    public interface ISession
    {
        string Id { get; }

        IObservable<ISessionFeed> RefreshFeed { get; }
    }
}
