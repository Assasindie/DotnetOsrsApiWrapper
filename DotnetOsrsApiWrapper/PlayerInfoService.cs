using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;

namespace DotnetOsrsApiWrapper
{
    public class PlayerInfoService
    {
        private readonly HttpClient _httpClient;

        public PlayerInfoService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<PlayerInfo> GetPlayerInfoAsync(string userName)
        {
            var playerInfo = new PlayerInfo();
            playerInfo.Name = userName;

            var response = await _httpClient.GetAsync("https://secure.runescape.com/m=hiscore_oldschool/index_lite.ws?player=" + userName);

            using var reader = new StreamReader(await response.Content.ReadAsStreamAsync());

            //request player info from jagex api
            try
            {
                //gets all the properties of the PlayerInfo class
                PropertyInfo[] properties = typeof(PlayerInfo).GetProperties();
                foreach (PropertyInfo info in properties)
                {
                    //checks the PropertyType of the current Property and sets the value accordingly.
                    if (info.PropertyType == typeof(Skill))
                    {
                        string[] values = reader.ReadLine().Split(',');
                        info.SetValue(playerInfo, new Skill
                        {
                            Name = info.Name,
                            Rank = int.Parse(values[0]),
                            Level = int.Parse(values[1]),
                            Experience = int.Parse(values[2])
                        });
                    }

                    if (info.PropertyType == typeof(Activity))
                    {
                        string[] values = reader.ReadLine().Split(',');
                        info.SetValue(playerInfo, new Activity
                        {
                            Name = info.Name,
                            Rank = int.Parse(values[0]),
                            Score = int.Parse(values[1])
                        });
                    }
                }
            } catch { }

            return playerInfo;
        }
    }
}
