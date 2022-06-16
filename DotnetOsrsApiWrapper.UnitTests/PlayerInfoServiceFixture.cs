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

namespace DotnetOsrsApiWrapper.UnitTests
{
    [TestClass]
    public class PlayerInfoServiceFixture
    {
        private HttpClient _httpClient;
        private PlayerInfoService _service;

        [TestInitialize]
        public void BeforeTest()
        {
            _httpClient = new HttpClient();

            _service = new PlayerInfoService(_httpClient);
        }

        [TestMethod]
        public async Task GetPlayerInfoAsync_ReturnsPlayerInfo()
        {
            var response = await _service.GetPlayerInfoAsync("Worstjibs");
        }

        [TestMethod]
        public async Task ExtensionMethod_WorksCorrectly()
        {
            var service = new ServiceCollection().AddOsrsWrapper()
                .BuildServiceProvider().GetRequiredService<IPlayerInfoService>();

            var response = await service.GetPlayerInfoAsync("Worstjibs");
        }

        [TestMethod]
        public async Task GetPlayerInfoAsync_ParsesCSVDataCorrectly()
        {
            // Arrange
            var randomSkills = GenerateSkillData();
            var randomActivities = GenerateActivityData();

            var csvString = GenerateCSV(randomSkills, randomActivities);

            var messageHander = new Mock<HttpMessageHandler>();
            messageHander.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .Returns((HttpRequestMessage request, CancellationToken cancellationToken) => GetResponseMessage(csvString));

            var service = new PlayerInfoService(new HttpClient(messageHander.Object));

            // Act
            var playerInfo = await service.GetPlayerInfoAsync("Worstjibs");

            // Assert
            var playerInfoSkills = playerInfo.Skills();
            Assert.AreEqual(randomSkills.Count(), playerInfoSkills.Count());
            foreach (var playerInfoSkill in playerInfoSkills)
            {
                var matchingSkill = randomSkills.Single(x => x.Name == playerInfoSkill.Name);
                Assert.AreEqual(matchingSkill.Level, playerInfoSkill.Level);
                Assert.AreEqual(matchingSkill.Rank, playerInfoSkill.Rank);
                Assert.AreEqual(matchingSkill.Experience, playerInfoSkill.Experience);
            }

            var playerInfoActivites = playerInfo.Minigames();
            Assert.AreEqual(randomActivities.Count(), playerInfoActivites.Count());
            foreach (var playerInfoActivity in playerInfoActivites)
            {
                var matchingActivity = randomActivities.Single(x => x.Name == playerInfoActivity.Name);
                Assert.AreEqual(matchingActivity.Score, playerInfoActivity.Score);
                Assert.AreEqual(matchingActivity.Rank, playerInfoActivity.Rank);
            }
        }

        private IEnumerable<Skill> GenerateSkillData()
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

        private IEnumerable<Activity> GenerateActivityData()
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