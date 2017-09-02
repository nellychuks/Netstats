using System;
using System.Collections.Generic;
using System.Linq;
using AngleSharp.Dom;
using AngleSharp.Dom.Html;
using AngleSharp.Parser.Html;
using Netstats.Network;
using System.Threading.Tasks;

namespace Netstats.Network
{
    internal static class HtmlPageMixins
    {
        private static Lazy<HtmlParser> lazyHtmlParser = new Lazy<HtmlParser>();

        public static HtmlParser SingletonHtmlParser { get { return lazyHtmlParser.Value; } }

        public static async Task<IHtmlDocument> ParsePageAsync(string html)
        {
            if (string.IsNullOrEmpty(html))
                throw new ArgumentException(html);

            return await SingletonHtmlParser.ParseAsync(html);
        }

        public static IEnumerable<IElement> GetAllSiblings(this IHtmlDocument document, string selector, string value)
        {
            if (document == null)
                return Enumerable.Empty<IElement>();
            return document.QuerySelectorAll(selector).Where(x => x.TextContent.ToLowerInvariant().Contains(value.ToLowerInvariant()))
                                                      .SelectMany(x => ((IElement)x.Parent).Children)
                                                      .Where(x => x.TextContent != value);
        }

        public static string GetSessionId(this IHtmlDocument document)
        {
            var meta = document.QuerySelectorAll("meta")
                               .Where(x => x.Attributes.Any(att => att.Name == "content" && att.Value.Contains("gi")))
                               .Select(elem => elem.Attributes["content"].Value)
                               .FirstOrDefault();
            return meta.Substring(meta.IndexOf("?gi=") + 4);
        }

        public static HtmlPageKind GetPageType(this IHtmlDocument document) => HtmlPageDescriptor.Identify(document);
    }
}
