using System;

namespace Netstats.Core.Interfaces
{
    public interface ISession 
    {
        string Id { get; }
        IObservable<ISessionFeed> RefreshFeed { get; }
    }
}
