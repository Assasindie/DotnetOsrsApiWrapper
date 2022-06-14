using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Net.Http;
using System.Threading.Tasks;

namespace DotnetOsrsApiWrapper.UnitTests
{
    [TestClass]
    public class PlayerInfoServiceFixture
    {
        private HttpClient _httpClient;
        private PlayerInfoService _service;

        private Mock<HttpMessageHandler> _mockMessageHandler;

        [TestInitialize]
        public void BeforeTest()
        {
            _mockMessageHandler = new Mock<HttpMessageHandler>();

            _httpClient = new HttpClient();

            _service = new PlayerInfoService(_httpClient);
        }

        [TestMethod]
        public async Task GetPlayerInfoAsync_ReturnsPlayerInfo()
        {
            var response = await _service.GetPlayerInfoAsync("Worstjibs");
        }
    }
}