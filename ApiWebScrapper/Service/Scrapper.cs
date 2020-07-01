using ApiWebScrapper.Service.Interface;
using HtmlAgilityPack;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ApiWebScrapper.Service
{
    /// <summary>
    /// Base class with common methods for scrapping web sites
    /// </summary>
    public abstract class Scrapper : IScrapper
    {
        protected string URL_BASE;

        /// <summary>
        /// Read the site from given URL to html document
        /// </summary>
        public async Task<HtmlDocument> GetSiteContents(string url)
        {
            string html;
            //Read the HTML content from the repository
            using (var client = new WebClient())
            {
                html = await client.DownloadStringTaskAsync(new Uri(url));
            }

            //Parser the html string contents
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            return doc;
        }

        /// <summary>
        /// Read the site from given URL to html document until find a specific text
        /// This is usefull to prevent load large pages if not needed all the content
        /// </summary>
        public async Task<HtmlDocument> GetSiteContents(string url, string textToFinish)
        {
            StringBuilder html = new StringBuilder();

            using (var client = new WebClient())
            {
                Stream stream = await client.OpenReadTaskAsync(new Uri(url));
                StreamReader sr = new StreamReader(stream);
                while (!sr.EndOfStream)
                {
                    string line = await sr.ReadLineAsync();
                    html.Append(line);
                    if (line.Contains(textToFinish)) break;
                }
            }

            //Parser the html string contents
            var doc = new HtmlDocument();
            doc.LoadHtml(html.ToString());

            return doc;
        }


        /// <summary>
        /// Verify if the URL in argument is the same of the base
        /// </summary>
        public bool ValidateSite(string url)
        {
            return url.StartsWith(URL_BASE);
        }

        public abstract Task<Tuple<string, string>> ValidateAndLoadSite(string url);
    }
}
