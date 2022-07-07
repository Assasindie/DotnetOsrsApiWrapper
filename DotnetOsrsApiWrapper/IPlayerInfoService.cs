using System.Collections.Generic;
using System.Threading.Tasks;

namespace DotnetOsrsApiWrapper
{
    public interface IPlayerInfoService
    {
        /// <summary>
        /// Return PlayerInfo for given Username
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        Task<PlayerInfo> GetPlayerInfoAsync(string userName);

        /// <summary>
        /// Return Enumerable of PlayerInfo for given Usernames
        /// </summary>
        /// <param name="userNames"></param>
        /// <returns></returns>
        Task<IEnumerable<PlayerInfo>> GetPlayerInfoAsync(string[] userNames);
    }
}