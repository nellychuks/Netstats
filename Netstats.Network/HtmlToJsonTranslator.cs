using AngleSharp.Parser.Html;
using Netstats.Core.Network;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Netstats.Network
{
    public class HtmlToJsonTranslator : IHtmlTranslator
    {
        HtmlParser SingletonHtmlParser { get; } = new HtmlParser();

        public async Task<string> Translate(string html, RequestAction action)
        {
            var page = await HtmlPageMixins.ParsePageAsync(html);
            
            switch(action)
            {
                case RequestAction.Login:
                    return TranslateLoginRequest(page);

                case RequestAction.Refresh:
                    return TranslateRefreshRequest(page);

                case RequestAction.Logout:
                    return TranslateLogoutRequest(page);

                default:
                    throw new Exception();
            }
        }
        
        #region Internals

        private string TranslateLoginRequest(IHtmlPage page)
        {
            throw new NotImplementedException();
        }

        private string TranslateLogoutRequest(IHtmlPage page)
        {
            throw new NotImplementedException();
        }

        private string TranslateConfirmLoginRequest(IHtmlPage page)
        {
            throw new NotImplementedException();
        }

        private string TranslateRefreshRequest(IHtmlPage page)
        {
            var graph = new
            {
                Succeded = true,

                TotalBandwidth = HtmlPageMixins.GetAllSiblings(page, "td.para1", "Group Allowed Bandwidth: ")
                                               .Select(x => x.TextContent)
                                               .FirstOrDefault(),

                UsedBandwidth = HtmlPageMixins.GetAllSiblings(page, "td.para1", "Total Bandwidth:")
                                               .Select(x => x.TextContent)
                                               .FirstOrDefault(),

                Download = HtmlPageMixins.GetAllSiblings(page, "td.para1", "Download:")
                                               .Select(x => x.TextContent)
                                               .FirstOrDefault(),

                Upload = HtmlPageMixins.GetAllSiblings(page, "td.para1", "Upload:")
                                           .Select(x => x.TextContent)
                                           .FirstOrDefault(),

                QuotaType = HtmlPageMixins.GetAllSiblings(page, "td.para1", "Bandwidth Quota Schedule:")
                                           .Select(x => x.TextContent)
                                           .FirstOrDefault(),
            };
            return JsonConvert.SerializeObject(graph, Formatting.Indented);
        }

        #endregion
    }
}
