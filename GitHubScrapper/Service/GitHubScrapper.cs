using ApiWebScrapper.Resources;
using ApiWebScrapper.ViewModel;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static ApiWebScrapper.Helpers.Enums;
using ByteSizeLib;
using System.Globalization;

namespace ApiWebScrapper.Service
{
    /// <summary>
    /// Git Hub Web Scrapper class
    /// </summary>
    public class GitHubScrapper : Scrapper
    {
        private List<GitHubResults> Results { get; set; }

        public int MyProperty { get; set; }
        public GitHubScrapper()
        {
            URL_BASE = "https://github.com";
            Results = new List<GitHubResults>();
        }

        /// <summary>
        /// Validate the repository and load its contents
        /// </summary>
        /// <param name="url"></param>
        /// <param name="document"></param>
        /// <returns></returns>
        public override async Task<Tuple<string, HtmlDocument>> ValidateAndLoadSite(string url)
        {
            string result = string.Empty;
            HtmlDocument document = new HtmlDocument();

            //Validate the URL 
            if (!ValidateSite(url))
                result = Messages.GitHubNotFound;
            else
            {
                //Proceed to load the contents of the repository
                document = await GetSiteContents(url);

                //Validate if contents is a valid repository
                bool isRepository = document.DocumentNode.Descendants("tbody")
                    .Select(y => y.Descendants("div")
                    .Where(x => x.Attributes["class"].Value == "repository-content")).Any();

                if(!isRepository) result = Messages.InvalidRepository;
            }

            return new Tuple<string, HtmlDocument>(result, document);
        }

        public async Task Navigate(string url, LinkType type)
        {
            //Necessary to append the base GitHub URL to the link of the element.
            if(!url.StartsWith(URL_BASE))
                url = URL_BASE + url;

            HtmlDocument doc;

            if (type == LinkType.Folder)
            {
                doc = await GetSiteContents(url);
                await ProcessDirectory(doc);
            }
            else
            {
                //A file of a repository can be very large. This prevent loading the file contents that is not necessary.
                //doc = await GetSiteContents(url, $"<div itemprop=\"text\"");
                doc = await GetSiteContents(url);
                ProcessFile(doc);
            }
        }

        /// <summary>
        /// Process a file and get the extention, filesize and line count
        /// </summary>
        /// <param name="doc"></param>
        public void ProcessFile(HtmlDocument doc)
        {
            //Get the filename and extention
            var fileName = doc.DocumentNode.Descendants("strong").FirstOrDefault(x => x.Attributes["class"].Value == "final-path").InnerText;

            //Get the filezie and line count  
            var stats = doc.DocumentNode.Descendants("div")
                .Last(x => x.OuterHtml.Contains("text-mono f6 flex-auto pr-3 flex-order-2 flex-md-order-1 mt-2 mt-md-0"))
                .InnerText.Trim().Split(" ").ToList();
            double bytes; int lines;

            if (stats.Count == 2 || stats.Any(x => x.Equals("LFS")))
            {
                //if the file is a binary type (like a jpg or zip file). 
                lines = 0;
                bytes = GetFileSize(stats[0], stats[1]);
            }
            else
            {
                //Text file, the line count is the first element and size the lasts elements
                lines = Convert.ToInt32(stats[0]);
                bytes = GetFileSize(stats[^2],stats[^1]);
            }

            //Save the data in the array of results
            GitHubResults result = new GitHubResults()
            {
                Extension = GetFileExtention(fileName),
                TotalBytes = bytes,
                TotalLines = lines
            };

            Results.Add(result);
        }

        public async Task ProcessDirectory(HtmlDocument doc)
        {
            //Get the table of files and folders
            var table = doc.DocumentNode.Descendants("tbody");

            //Get the icons information (to determine if is a file or a folder)
            var icons = table.Select(y => y.Descendants("td")
                .Where(x => x.Attributes["class"].Value == "icon")).FirstOrDefault().ToList();

            //Get the file name (to extract the extension)
            var content = table.Select(y => y.Descendants("td")
                .Where(x => x.Attributes["class"].Value == "content")).FirstOrDefault().ToList();

            for (int i = 1; i < icons.Count; i++)
            {
                //The first element of the table is not necessary
                bool isDirectory = icons[i].InnerHtml.Contains("directory");

                //Get the URL to navigate to the item
                string urlItem = content[i].Descendants()
                    .FirstOrDefault(x => x.Name.Equals("a")).Attributes
                    .FirstOrDefault(a => a.Name.Equals("href")).Value;

                var type = isDirectory ? LinkType.Folder : LinkType.File;

                await Navigate(urlItem, type);
            }
        }

        /// <summary>
        /// Process the results grouping by extension
        /// </summary>
        public List<GitHubResults> FormatResults()
        {
            List<GitHubResults> formattedResults = new List<GitHubResults>();

            foreach (var item in Results)
            {
                var existingItem = formattedResults.FirstOrDefault(x => x.Extension == item.Extension);
                if (existingItem == null)
                {
                    formattedResults.Add(item);
                }
                else
                {
                    existingItem.TotalBytes += item.TotalBytes;
                    existingItem.TotalLines += item.TotalLines;
                }
            }

            return formattedResults;
        }

        #region Private methods
        /// <summary>
        /// Get the extension of the filename.
        /// </summary>
        private string GetFileExtention(string fileName)
        {
            return fileName.Split(".").Last();
        }

        /// <summary>
        /// Convert the file size info from the repository to bytes
        /// </summary>
        /// <param name="value">raw value</param>
        /// <param name="unit">indicate if the value is a Byte, Kbyte or Mbyte</param>
        /// <returns></returns>
        private double GetFileSize(string value, string unit)
        {
            double result = double.Parse(value, CultureInfo.InvariantCulture);

            result = unit switch
            {
                "Bytes" => ByteSize.FromBytes(result).Bytes,
                "KB" => ByteSize.FromKibiBytes(result).Bytes,
                "MB" => ByteSize.FromMebiBytes(result).Bytes,
                _ => 0,
            };

            return Math.Truncate(result);
        }
        #endregion
    }
}
