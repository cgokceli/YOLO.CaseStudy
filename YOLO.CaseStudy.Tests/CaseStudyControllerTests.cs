using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using YOLO.CaseStudy.API.Controllers;
using YOLO.CaseStudy.Business.Interfaces;
using YOLO.CaseStudy.Entities;

namespace YOLO.CaseStudy.Tests
{
    public class CaseStudyControllerTests
    {
        private static Result GetTestData()
        {
            return new SuccessResult<IEnumerable<int>>(Enumerable.Range(1, 100));
        }

        [Fact]
        public async Task Iterator_ReturnsAResult_WithAListOfIntegers()
        {
            // Setup
            var mockRepo = new Mock<ICaseStudyBusiness>();
            mockRepo.Setup(repo => repo.Iterator(default)).ReturnsAsync(GetTestData());
            var controller = new CaseStudyController(mockRepo.Object);

            // Invoke
            var result = await controller.Iterator(default);

            // Assert
            var apiResult = Assert.IsType<SuccessResult<IEnumerable<int>>>(result);
            var data = Assert.IsAssignableFrom<IEnumerable<int>>(apiResult.Data);
            Assert.Equal(100, data.Count());
        }
    }
}
