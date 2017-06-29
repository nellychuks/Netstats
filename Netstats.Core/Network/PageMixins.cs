using System;
using System.Collections.Generic;
using System.Linq;
using AngleSharp.Dom;
using AngleSharp.Dom.Html;
using AngleSharp.Parser.Html;


namespace Netstats.Core.Network
{
    public static class PageMixins
    {
        private static Lazy<HtmlParser> lazyHtmlParser = new Lazy<HtmlParser>();
        public static HtmlParser SingletonHtmlParser { get { return lazyHtmlParser.Value; } }

        public static IPage ParsePage(string pageContent)
        {
            if (string.IsNullOrEmpty(pageContent))
                throw new ArgumentException(pageContent);

            var htmlContent = SingletonHtmlParser.Parse(pageContent);
            var kind = PageDescriptor.Instance.Identify(htmlContent);
            return new GenericPage(htmlContent, kind);
        }

        public static IEnumerable<IElement> GetAllSiblings(IHtmlDocument document, string selector, string value)
        {
            if (document == null)
                return Enumerable.Empty<IElement>();
            return document.QuerySelectorAll(selector).Where(x => x.TextContent.ToLowerInvariant().Contains(value.ToLowerInvariant()))
                                                      .SelectMany(x => ((IElement)x.Parent).Children)
                                                      .Where(x => x.TextContent != value);
        }

        public static string ParseId(IPage page)
        {
            var meta = page.Content.QuerySelectorAll("meta")
               .Where(x => x.Attributes.Any(att => att.Name == "content" && att.Value.Contains("gi")))
               .Select(elem => elem.Attributes["content"].Value)
               .FirstOrDefault();
            return meta.Substring(meta.IndexOf("?gi=") + 4);
        }
    }
}
