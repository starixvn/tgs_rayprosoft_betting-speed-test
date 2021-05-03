using System.Threading.Tasks;
using TGS.BettingSpeedTest.Messages.Global;

namespace TGS.BettingSpeedTest.GameProviderApis
{
    public interface IGameProviderApi
    {
        string SetupHttpsForEndpoint(bool enableHttps);

        ValueTask PlaceSingleBet(string userId, CreatePlayerAndLoginResponse createPlayerAndLoginResponse, int count);

        ValueTask PlaceMultipleBets(string userId, CreatePlayerAndLoginResponse createPlayerAndLoginResponse,
            int count);

        ValueTask PlaceDuplicateSingleBet(string userId, CreatePlayerAndLoginResponse createPlayerAndLoginResponse,
            int count);

        ValueTask PlaceOppositeBets(string userId, CreatePlayerAndLoginResponse createPlayerAndLoginResponse,
            int count);
    }
}