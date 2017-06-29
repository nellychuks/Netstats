using AngleSharp.Dom.Html;

namespace Netstats.Core.Network
{
    public struct GenericPage : IPage
    {
        public GenericPage(IHtmlDocument pageContent, PageKind kind)
        {
            Content = pageContent;
            Kind = kind;
        }

        public IHtmlDocument Content { get; }
        public PageKind Kind { get; }

        public void Dispose()
        {
            Content.Dispose();
        }
    }

    public enum PageKind
    {
        UnknownPage,

        SessionPage,

        AuthenticationFailedPage,

        LoggedOutPage,

        ConfirmationPage,

        BandwidthExceededPage,

        MaxUserSessionsReached
    }
}
