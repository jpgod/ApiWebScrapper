using ApiWebScrapper.Controllers;
using ApiWebScrapper.ViewModel;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Xunit;
using System.Linq;

namespace ApiWebScrapper.Test
{
    public class GitHubScrapperTest
    {
        /// <summary>
        /// Test another repository i created
        /// </summary>
        [Fact]
        public async Task TestAPI()
        {
            // Arrange
            var testURL = new Repository{ Url = "https://github.com/jpgod/TestSite" };

            // Act  
            var controller = new GitHubController();

            var actionResult = await controller.PostParser(testURL);

            // Assert
            var viewResult = Assert.IsType<ObjectResult>(actionResult);
            var model = Assert.IsAssignableFrom<IEnumerable<GitHubResults>>(viewResult.Value);

            Assert.Equal(13, model.Count());

            var testSLN = model.FirstOrDefault(x => x.Extension.Equals(".sln"));

            Assert.Equal(1578, testSLN.TotalBytes);
            Assert.Equal(31, testSLN.TotalLines);
        }

        /// <summary>
        /// Test a bigger repository "Boleto.NET"
        /// </summary>
        [Fact]
        public async Task TestAPI2()
        {
            // Arrange
            var testURL = new Repository { Url = "https://github.com/BoletoNet/boletonet" };

            // Act  
            var controller = new GitHubController();

            var actionResult = await controller.PostParser(testURL);

            // Assert
            var viewResult = Assert.IsType<ObjectResult>(actionResult);
            var model = Assert.IsAssignableFrom<IEnumerable<GitHubResults>>(viewResult.Value);

            Assert.Equal(44, model.Count());

            var testSLN = model.FirstOrDefault(x => x.Extension.Equals(".sln"));

            Assert.Equal(8692, testSLN.TotalBytes);
            Assert.Equal(137, testSLN.TotalLines);
        }
    }
}
