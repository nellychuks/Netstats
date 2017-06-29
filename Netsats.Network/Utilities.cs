using AngleSharp.Dom;
using System;
using System.Collections.Generic;
using System.Linq;
using AngleSharp.Dom.Html;
using AngleSharp.Parser.Html;
using Netstats.Network.Api;

namespace Netstats.Network
{
    public class Utilities
    {
        static Dictionary<PageKind, Func<IHtmlDocument, bool>> PageIdentifiers = new Dictionary<PageKind, Func<IHtmlDocument, bool>>()
        {
            [PageKind.SessionPage] = x => GetAllElements(x, "p.para1a").Any(elem => elem.TextContent == "Note : If your browser is inactive for more than 23 Hours, then you will be logged out."),

            [PageKind.AuthenticationFailedPage] = doc => GetAllElements(doc, "p").Any(elem => elem.TextContent == "Authentication Failed"),

            [PageKind.ConfirmationPage] = doc => GetAllWithAttribute(doc, "form", "name").Any(elem => elem.Attributes["name"].Value == "confirmaction"),

            [PageKind.LoggedOutPage] = doc => GetAllElements(doc, "p").Any(elem => elem.TextContent == "You have been successfully Logged Out!!!")
        };

        public static HtmlParser SingletonHtmlParser { get { return LazySingletonParser.Value; } }


        static Lazy<HtmlParser> LazySingletonParser = new Lazy<HtmlParser>();

        public static IEnumerable<IElement> GetAllSiblings(IHtmlDocument document, string selector, string value)
        {
            return document.QuerySelectorAll(selector).Where(x => x.TextContent.ToLowerInvariant().Contains(value.ToLowerInvariant()))
                                                      .SelectMany(x => ((IElement)x.Parent).Children)
                                                      .Where(x => x.TextContent != value);
        }

        public static IEnumerable<IElement> GetAllWithAttribute(IHtmlDocument document, string selector, string attribute)
        {
            return GetAllElements(document, selector).Where(x => x.Attributes.Any(att => att.Name.ToLowerInvariant() == attribute.ToLowerInvariant()));
        }

        public static IEnumerable<IElement> GetAllElements(IHtmlDocument document, string selector)
        {
            return document.QuerySelectorAll(selector);
        }

        public static string ParseSessionId(IHtmlDocument document)
        {
            var meta = GetAllWithAttribute(document, "meta", "content").Where(elem => elem.Attributes["content"].Value.Contains("gi"))
                                                                       .Select(elem => elem.Attributes["content"].Value)
                                                                       .FirstOrDefault();
            return meta.Substring(meta.IndexOf("?gi=") + 4);
        }

        public static SessionFeed ParseSessionFeed(IHtmlDocument page)
        {
            var download = GetAllSiblings(page, "td.para1", "Download:").Select(x => x.TextContent).FirstOrDefault();
            var upload = GetAllSiblings(page, "td.para1", "Upload:").Select(x => x.TextContent).FirstOrDefault();
            var quotaType = GetAllSiblings(page, "td.para1", "Bandwidth Quota Schedule:").Select(x => x.TextContent).FirstOrDefault();
            var total = GetAllSiblings(page, "td.para1", "Group Allowed Bandwidth: ").Select(x => x.TextContent).FirstOrDefault();

            return new SessionFeed()
            {
                QuotaType = (BandwidthQuotaType)Enum.Parse(typeof(BandwidthQuotaType), quotaType),
                TotalBandwidth = ParseToMb(total),
                Download = ParseToMb(download),
                Upload = ParseToMb(upload),
            };
        }

        public static double ParseToMb(string text)
        {
            if (text.ToLowerInvariant().Contains("gb"))
                return double.Parse((text.Substring(0, text.Length - text.ToLowerInvariant().IndexOf("gb")))) * 1024d;
            else
                return double.Parse((text.Substring(0, text.Length - text.ToLowerInvariant().IndexOf("mb"))));
        }

        public static double FormatToGB(double number) => number >= 1024 ? Math.Round(number / 1024f, 2) : Math.Round(number, 2);

        public static string FormatToString(double number) => number >= 1024 ? $"{Math.Round(number / 1024f, 2)}GB" : $"{Math.Round(number, 2)}MB";

        public static PageKind IdentifyPage(IHtmlDocument document)
        {
            foreach (var pageType in PageIdentifiers.Keys)
                if (PageIdentifiers[pageType](document))
                    return pageType;
            return PageKind.UnknownPage;
        }
    }
}