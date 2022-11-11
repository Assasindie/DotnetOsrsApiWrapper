using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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
            var playerInfo = new PlayerInfo { Name = userName };

            var response = await _httpClient.GetAsync("https://secure.runescape.com/m=hiscore_oldschool/index_lite.ws?player=" + userName)
                .ConfigureAwait(false);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                playerInfo.Status = PlayerInfoStatus.NotFound;
                return playerInfo;
            }

            return await ParseHttpResponse(playerInfo, response)
                .ConfigureAwait(false);
        }

        public async Task<IEnumerable<PlayerInfo>> GetPlayerInfoAsync(string[] userNames)
        {
            var tasks = userNames.Select(x => GetPlayerInfoAsync(x));
            return await Task.WhenAll(tasks)
                .ConfigureAwait(false);
        }

        private async Task<PlayerInfo> ParseHttpResponse(PlayerInfo playerInfo, HttpResponseMessage response)
        {
            var stream = await response.Content.ReadAsStreamAsync()
                .ConfigureAwait(false);

            using var reader = new StreamReader(stream);

            var properties = typeof(PlayerInfo).GetProperties()
                .Where(x => typeof(IPlayerInfoProperty).IsAssignableFrom(x.PropertyType)).ToArray();

            if (properties.Count() != TotalLines(reader) && _throwMismatchException)
                throw new FormatException("Property mismatch; OSRS API Contains more properties than PlayerInfo class. Please contact Repository creator");

            foreach (PropertyInfo info in properties)
            {
                string[] values = reader.ReadLine().Split(',');
                var property = ParseLine(values, info);

                info.SetValue(playerInfo, property);
            }

            playerInfo.Status = PlayerInfoStatus.Success;

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
