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
        private bool _throwMismatchException;

        internal bool ThrowMismatchException
        {
            get => _throwMismatchException;
            set => _throwMismatchException = value;
        }

        public PlayerInfoService(HttpClient httpClient, bool throwMismatchException = true)
        {
            _httpClient = httpClient;
            _throwMismatchException = throwMismatchException;
        }

        public async Task<PlayerInfo> GetPlayerInfoAsync(string userName)
        {
            var response = await _httpClient.GetAsync("https://secure.runescape.com/m=hiscore_oldschool/index_lite.ws?player=" + userName);
            response.EnsureSuccessStatusCode();

            return await ParseHttpResponse(userName, response);
        }

        public async Task<IEnumerable<PlayerInfo>> GetPlayerInfoAsync(string[] userNames)
        {
            var tasks = userNames.Select(x => GetPlayerInfoAsync(x));
            return await Task.WhenAll(tasks);
        }

        private async Task<PlayerInfo> ParseHttpResponse(string userName, HttpResponseMessage response)
        {
            var playerInfo = new PlayerInfo { Name = userName };

            var stream = await response.Content.ReadAsStreamAsync();

            using var reader = new StreamReader(stream);

            var properties = typeof(PlayerInfo).GetProperties()
                .Where(x => new[] { typeof(Activity), typeof(Skill) }.Contains(x.PropertyType));

            if (properties.Count() != TotalLines(reader) && _throwMismatchException)
                throw new FormatException("Property mismatch; OSRS API Contains more properties than PlayerInfo class. Please contact Repository creator");

            foreach (PropertyInfo info in properties)
            {
                string[] values = reader.ReadLine().Split(',');
                var property = ParseLine(values, info);

                info.SetValue(playerInfo, property);
            }

            return playerInfo;
        }

        private int TotalLines(StreamReader reader)
        {
            int i = 0;
            while (reader.ReadLine() != null) i++;

            reader.BaseStream.Position = 0;

            return i;
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
