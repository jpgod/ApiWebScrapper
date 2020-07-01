using System;
using System.Threading.Tasks;
using ApiWebScrapper.Service;
using ApiWebScrapper.ViewModel;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiWebScrapper.Controllers
{
    [ApiController, Route("api/[controller]")]
    public class GitHubController : ControllerBase
    {
        public GitHubController() {}

        /// <summary>
        /// API endpoint to scrapper the GitHub repository
        /// POST /api/parser
        /// </summary>
        /// <param name="url">GitHub repository link</param>
        /// <returns>JSON with the results</returns>
        [HttpPost]
        public async Task<IActionResult> PostParser(Repository url)
        {
            var parser = new GitHubScrapper();

            //Validate if url is a valid github repository
            Tuple<string, string> validate = await parser.ValidateAndLoadSite(url.Url);
            if (!string.IsNullOrEmpty(validate.Item1)) return BadRequest(validate.Item1);

            try
            {
                //Process the zip file with the repository contents
                parser.ProcessZipFile(validate.Item2);

                //Get the results
                var results = parser.FormatResults();

                return new ObjectResult(results);
            }
            catch (Exception ex)
            {
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
