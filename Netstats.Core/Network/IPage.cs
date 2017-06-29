using System;
using AngleSharp.Dom.Html;

namespace Netstats.Core.Network
{
    public interface IPage: IDisposable
    {
        PageKind Kind { get; }
        IHtmlDocument Content { get;  }
    }
}
