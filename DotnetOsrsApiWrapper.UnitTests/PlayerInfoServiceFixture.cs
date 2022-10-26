using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace DotnetOsrsApiWrapper.UnitTests
{
    [TestClass]
    public class PlayerInfoServiceFixture
    {
        private Mock<HttpMessageHandler> _messageHandler;
        private HttpClient _httpClient;

        private PlayerInfoService _service;

        [TestInitialize]
        public void BeforeTest()
        {
            _messageHandler = new Mock<HttpMessageHandler>();
            _httpClient = new HttpClient(_messageHandler.Object);

            _service = new PlayerInfoService(_httpClient, true);
        }

        [TestMethod]
        public void ExtensionMethod_WorksCorrectly()
        {
            var service = new ServiceCollection().AddOsrsWrapper()
                .BuildServiceProvider().GetRequiredService<IPlayerInfoService>();

            Assert.IsNotNull(service);
            Assert.IsTrue(service is PlayerInfoService);
        }

        [TestMethod]
        public async Task GetPlayerInfoAsync_ParsesCSVDataCorrectly()
        {
            // Arrange
            var randomSkills = GenerateSkillData();
            var randomActivities = GenerateActivityData();

            var csvString = GenerateCSV(randomSkills, randomActivities);

            _messageHandler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .Returns((HttpRequestMessage request, CancellationToken cancellationToken) => GetResponseMessage(csvString));

            var userName = "Worstjibs";

            // Act
            var playerInfo = await _service.GetPlayerInfoAsync(userName);

            // Assert
            Assert.AreEqual(userName, playerInfo.Name);
            CheckPlayerInfo(randomSkills, randomActivities, playerInfo);
        }

        [TestMethod]
        public async Task GetPlayerInfoAsync_SetsStatusToSuccess()
        {
            // Arrange
            var randomSkills = GenerateSkillData();
            var randomActivities = GenerateActivityData();

            var csvString = GenerateCSV(randomSkills, randomActivities);

            _messageHandler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .Returns((HttpRequestMessage request, CancellationToken cancellationToken) => GetResponseMessage(csvString));

            var userName = "Worstjibs";

            // Act
            var playerInfo = await _service.GetPlayerInfoAsync(userName);

            // Assert
            Assert.AreEqual(PlayerInfoStatus.Success, playerInfo.Status);
        }

        [TestMethod]
        public async Task GetPlayerInfoAsync_SetsStatusToNotFound_WhenHttpClientReturnsNotFound()
        {
            // Arrange
            _messageHandler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .Returns((HttpRequestMessage request, CancellationToken cancellationToken) => Task.FromResult(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.NotFound
                }));

            // Act
            var result = await _service.GetPlayerInfoAsync("Worstjibs");

            // Assert
            Assert.AreEqual(PlayerInfoStatus.NotFound, result.Status);
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public async Task GetPlayerInfoAsync_ThrowsAnException_WhenPropertyMismatchOccurs(bool throwException)
        {
            // This test aims to relicate a scenario in which Jagex adds a new Activity or Skill to the Hiscores

            // Arrange
            var randomSkills = GenerateSkillData();
            var randomActivities = GenerateActivityData();
            randomActivities.Add(new Activity { Name = "Wintertodt 2" }); // Add a new Activity

            _service.ThrowMismatchException = throwException;

            var csvString = GenerateCSV(randomSkills, randomActivities);

            _messageHandler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .Returns((HttpRequestMessage request, CancellationToken cancellationToken) => GetResponseMessage(csvString));

            // Act / Assert
            if (throwException)
            {
                await Assert.ThrowsExceptionAsync<FormatException>(() => _service.GetPlayerInfoAsync("Worstjibs"));
            } else
            {
                Assert.IsNotNull(await _service.GetPlayerInfoAsync("Worstjibs"));
            }
        }

        [TestMethod]
        public async Task GetPlayerInfo_ReturnsEnumerableOfPlayerInfo()
        {
            // Arrange
            var userNames = new[] { "Worstjibs", "Bestjibs", "Normaljibs" };
            var playerDictionary = userNames.ToDictionary(x => x, x =>
            {
                return (GenerateSkillData(), GenerateActivityData());
            });

            foreach (var item in playerDictionary)
            {
                _messageHandler.Protected()
                    .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                        .Returns((HttpRequestMessage request, CancellationToken cancellationToken) =>
                        {
                            var playerUsername = HttpUtility.ParseQueryString(request.RequestUri.Query)["player"];
                            var playerData = playerDictionary[playerUsername];
                            return GetResponseMessage(GenerateCSV(playerData.Item1, playerData.Item2));
                        });
            }

            // Act
            var playerInfos = await _service.GetPlayerInfoAsync(userNames);

            // Assert
            Assert.AreEqual(userNames.Count(), playerInfos.Count());
            foreach (var playerInfo in playerInfos)
            {
                var expectedPlayer = playerDictionary[playerInfo.Name];
                Assert.IsNotNull(expectedPlayer);

                CheckPlayerInfo(expectedPlayer.Item1, expectedPlayer.Item2, playerInfo);
            }
        }

        private void CheckPlayerInfo(IEnumerable<Skill> expectedSkills, IEnumerable<Activity> expectedActivities, PlayerInfo actualPlayerInfo)
        {

            var playerInfoSkills = actualPlayerInfo.Skills();
            Assert.AreEqual(expectedSkills.Count(), playerInfoSkills.Count());
            foreach (var playerInfoSkill in playerInfoSkills)
            {
                var expectingSkill = expectedSkills.Single(x => x.Name == playerInfoSkill.Name);
                Assert.AreEqual(expectingSkill.Level, playerInfoSkill.Level);
                Assert.AreEqual(expectingSkill.Rank, playerInfoSkill.Rank);
                Assert.AreEqual(expectingSkill.Experience, playerInfoSkill.Experience);
            }

            var playerInfoActivites = actualPlayerInfo.Minigames();
            Assert.AreEqual(expectedActivities.Count(), playerInfoActivites.Count());
            foreach (var playerInfoActivity in playerInfoActivites)
            {
                var expectedActivity = expectedActivities.Single(x => x.Name == playerInfoActivity.Name);
                Assert.AreEqual(expectedActivity.Score, playerInfoActivity.Score);
                Assert.AreEqual(expectedActivity.Rank, playerInfoActivity.Rank);
            }
        }

        private List<Skill> GenerateSkillData()
        {
            var random = new Random();

            return typeof(PlayerInfo).GetProperties()
                .Where(x => x.PropertyType == typeof(Skill))
                .Select(x => new Skill
                {
                    Name = x.Name,
                    Experience = random.Next(1000000),
                    Level = random.Next(99) + 1,
                    Rank = random.Next(100000)
                }).ToList();
        }

        private List<Activity> GenerateActivityData()
        {
            var random = new Random();

            return typeof(PlayerInfo).GetProperties()
                .Where(x => x.PropertyType == typeof(Activity))
                .Select(x => new Activity
                {
                    Name = x.Name,
                    Rank = random.Next(100000),
                    Score = random.Next(5000)
                }).ToList();
        }

        private string GenerateCSV(IEnumerable<Skill> skills, IEnumerable<Activity> activities)
        {
            var stringBuilder = new StringBuilder();

            foreach (var skill in skills)
            {
                stringBuilder.AppendLine($"{skill.Rank},{skill.Level},{skill.Experience}");
            }

            foreach (var activity in activities)
            {
                stringBuilder.AppendLine($"{activity.Rank},{activity.Score}");
            }

            return stringBuilder.ToString();
        }

        private Task<HttpResponseMessage> GetResponseMessage(string content)
        {
            var response = new HttpResponseMessage
            {
                Content = new StringContent(content),
                StatusCode = HttpStatusCode.OK
            };
            return Task.FromResult(response);
        }
    }
}