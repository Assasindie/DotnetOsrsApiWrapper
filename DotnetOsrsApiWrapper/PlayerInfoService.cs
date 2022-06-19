using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;

namespace DotnetOsrsApiWrapper
{
    public class PlayerInfoService : IPlayerInfoService
    {
        private readonly HttpClient _httpClient;

        public PlayerInfoService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<PlayerInfo> GetPlayerInfoAsync(string userName)
        {
            var response = await _httpClient.GetAsync("https://secure.runescape.com/m=hiscore_oldschool/index_lite.ws?player=" + userName);
            response.EnsureSuccessStatusCode();

            return await ParseHttpResponse(userName, response);
        }

        public async Task<IEnumerable<PlayerInfo>> GetPlayerInfoAsync(string[] userNames)
        {
            var playerInfoList = new List<PlayerInfo>();

            foreach (var userName in userNames)
            {
                var response = await _httpClient.GetAsync("https://secure.runescape.com/m=hiscore_oldschool/index_lite.ws?player=" + userName);
                response.EnsureSuccessStatusCode();

                playerInfoList.Add(await ParseHttpResponse(userName, response));
            }

            return playerInfoList;
        }

        private async Task<PlayerInfo> ParseHttpResponse(string userName, HttpResponseMessage response)
        {
            var playerInfo = new PlayerInfo { Name = userName };

            using var reader = new StreamReader(await response.Content.ReadAsStreamAsync());

            try
            {
                var properties = typeof(PlayerInfo).GetProperties()
                    .Where(x => new[] { typeof(Activity), typeof(Skill) }.Contains(x.PropertyType));

                foreach (PropertyInfo info in properties)
                {
                    string[] values = reader.ReadLine().Split(',');
                    var property = ParseLine(values, info);

                    info.SetValue(playerInfo, property);
                }
            } catch { }

            return playerInfo;
        }

        private IPlayerInfoProperty ParseLine(string[] data, PropertyInfo info)
        {
            if (info.PropertyType == typeof(Skill))
            {
                return new Skill
                {
                    Name = info.Name,
                    Rank = int.Parse(data[0]),
                    Level = int.Parse(data[1]),
                    Experience = int.Parse(data[2])
                };
            }

            if (info.PropertyType == typeof(Activity))
            {
                return new Activity
                {
                    Name = info.Name,
                    Rank = int.Parse(data[0]),
                    Score = int.Parse(data[1])
                };
            }

            throw new NotImplementedException($"Unimplemented type {info.PropertyType}");
        }
    }
}
