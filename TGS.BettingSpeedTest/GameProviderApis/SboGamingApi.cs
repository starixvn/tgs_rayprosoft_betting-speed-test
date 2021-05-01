using System;
using System.Threading.Tasks;
using NLog;
using TGS.BettingSpeedTest.Clients;
using TGS.BettingSpeedTest.Messages.Global;
using TGS.BettingSpeedTest.Messages.Sbo;
using TGS.BettingSpeedTest.Settings;

namespace TGS.BettingSpeedTest.GameProviderApis
{
    public sealed class SboGamingApi : IGameProviderApi
    {
        private static readonly ISboApiClient SboApiClient = new SboApiClient();
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        string IGameProviderApi.SetupHttpsForEndpoint(bool enableHttps)
        {
            SboSettings.ApiEndpoint =
                string.Format(SboSettings.ApiEndpoint, enableHttps ? "https" : "http");
            return SboSettings.ApiEndpoint;
        }

        async ValueTask IGameProviderApi.PlaceSingleBet(string userId,
            CreatePlayerAndLoginResponse createPlayerAndLoginResponse, int count)
        {
            var playerCode = createPlayerAndLoginResponse.PlayerCode;

            var externalRoundId = $"{userId}-round-{DateTime.Now:yyMMddHHmmss}-{count}";
            var placeBetResponse =
                await SboApiClient.PlaceBetAsync(playerCode, externalRoundId);

            if (placeBetResponse == null || placeBetResponse.ErrorCode != 0)
            {
                Logger.Warn($"Player {userId} failed to place a bet");
                await Task.Delay(1000);
                return;
            }

            Logger.Info($"Player {userId} placed a bet");

            if (count % 5 == 0)
            {
                Logger.Info($"Player {userId} settling a bet");
                var settleBetResponse =
                    await SboApiClient.SettleBetAsync(playerCode, externalRoundId, ResultType.Win, 10);

                if (settleBetResponse == null)
                {
                    Logger.Warn($"Player {userId} failed to settle a bet");
                    return;
                }

                Logger.Info($"Player {userId} settled a bet");
            }
            else
            {
                Logger.Info($"Player {userId} losing a bet");
                var loseBetResponse =
                    await SboApiClient.SettleBetAsync(playerCode, externalRoundId, ResultType.Loss, 0);

                if (loseBetResponse == null)
                {
                    Logger.Warn($"Player {userId} failed to lose a bet");
                    return;
                }

                Logger.Info($"Player {userId} lost a bet");
            }
        }

        ValueTask IGameProviderApi.PlaceDuplicateSingleBet(string userId,
            CreatePlayerAndLoginResponse createPlayerAndLoginResponse, int count) =>
            throw new NotImplementedException();

        ValueTask IGameProviderApi.PlaceMultipleBets(string userId,
            CreatePlayerAndLoginResponse createPlayerAndLoginResponse, int count) =>
            throw new NotImplementedException();

        ValueTask IGameProviderApi.PlaceOppositeBets(string userId,
            CreatePlayerAndLoginResponse createPlayerAndLoginResponse, int count) =>
            throw new NotImplementedException();
    }
}