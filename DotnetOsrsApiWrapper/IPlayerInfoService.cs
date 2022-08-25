using System.Collections.Generic;
using System.Threading.Tasks;

namespace DotnetOsrsApiWrapper
{
    public interface IPlayerInfoService
    {
        /// <summary>
        /// Return PlayerInfo for given Username
        /// </summary>
        /// <param name="userName">The account name to query the hiscores</param>
        /// <returns>A PlayerInfo object associated with the account</returns>
        /// <exception cref="System.FormatException">Thrown when service is configured to throw on property mismatch.</exception>
        Task<PlayerInfo> GetPlayerInfoAsync(string userName);

        /// <summary>
        /// Return Enumerable of PlayerInfo for given Usernames
        /// </summary>
        /// <param name="userNames">An array of account names to query the hiscores</param>
        /// <returns>An Enumerable of PlayerInfo objects associated with the given account names</returns>
        /// <exception cref="System.FormatException">Thrown when service is configured to throw on property mismatch.</exception>
        Task<IEnumerable<PlayerInfo>> GetPlayerInfoAsync(string[] userNames);
    }
}