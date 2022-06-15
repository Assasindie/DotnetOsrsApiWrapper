using System.Threading.Tasks;

namespace DotnetOsrsApiWrapper
{
    public interface IPlayerInfoService
    {
        Task<PlayerInfo> GetPlayerInfoAsync(string userName);
    }
}