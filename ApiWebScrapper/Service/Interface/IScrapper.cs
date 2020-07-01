using HtmlAgilityPack;
using System;
using System.Threading.Tasks;

namespace ApiWebScrapper.Service.Interface
{
    interface IScrapper
    {
        public Task<HtmlDocument> GetSiteContents(string url);

        public Task<HtmlDocument> GetSiteContents(string url, string textToFinish);

        public bool ValidateSite(string url);

        public Task<Tuple<string, string>> ValidateAndLoadSite(string url);
    }
}
