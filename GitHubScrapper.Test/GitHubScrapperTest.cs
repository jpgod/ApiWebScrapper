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
        [Fact]
        public async Task TestAPI()
        {
            // Arrange
            var testURL = new Repository{ Url = "https://github.com/jpgod/ITIXteste" };

            // Act  
            var controller = new GitHubController();

            var actionResult = await controller.PostParser(testURL);

            // Assert
            var viewResult = Assert.IsType<ObjectResult>(actionResult);
            var model = Assert.IsAssignableFrom<IEnumerable<GitHubResults>>(viewResult.Value);

            Assert.Equal(13, model.Count());

            var testSLN = model.FirstOrDefault(x => x.Extension.Equals("sln"));

            Assert.Equal(1576, testSLN.TotalBytes);
            Assert.Equal(31, testSLN.TotalLines);
        }
    }
}
