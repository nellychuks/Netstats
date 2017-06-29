using System;

namespace Netstats.Network.Services
{
    public interface IRefreshService
    {
        TimeSpan RefreshInterval { get; }
        IObservable<SessionFeed> UpdateFeed { get; }
    }
}