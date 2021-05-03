using System;
using System.Linq;
using System.Threading.Tasks;
using NLog;
using TGS.BettingSpeedTest.Clients;
using TGS.BettingSpeedTest.Messages.Global;
using TGS.BettingSpeedTest.Settings;

namespace TGS.BettingSpeedTest.GameProviderApis
{
    public sealed class AsiaGamingApi : IGameProviderApi
    {
        private static readonly IAsiaGamingApiClient AsiaGamingApiClient = new AsiaGamingApiClient();
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        string IGameProviderApi.SetupHttpsForEndpoint(bool enableHttps)
        {
            AsiaGamingSettings.ApiEndpoint =
                string.Format(AsiaGamingSettings.ApiEndpoint, enableHttps ? "https" : "http");
            return AsiaGamingSettings.ApiEndpoint;
        }

        async ValueTask IGameProviderApi.PlaceMultipleBets(string userId,
            CreatePlayerAndLoginResponse createPlayerAndLoginResponse, int count)
        {
            var playerCode = createPlayerAndLoginResponse.PlayerCode;
            var loginRecordId = createPlayerAndLoginResponse.LoginRecordId;

            var externalRoundId = $"{userId}-Place-{DateTime.Now:yyMMddHHmmss}-{count}";
            var requests = Enumerable.Range(0, 3).Select(_ => AsiaGamingApiClient.PlaceBetAsync(
                loginRecordId, playerCode, externalRoundId).AsTask()).ToList();

            var responses = await Task.WhenAll(requests);

            if (responses.First().ErrorCode != AsiaGamingSettings.SuccessfulErrorCode)
            {
                Logger.Warn($"Player {userId} failed to place a bet");
                await Task.Delay(1000);
                return;
            }

            Logger.Info($"Player {userId} placed a bet");

            if (count % 5 == 0)
            {
                Logger.Info($"Player {userId} settling a bet");

                var settleBetResponse = await AsiaGamingApiClient.SettleBetAsync(
                    loginRecordId, playerCode, externalRoundId, 10);

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

                var loseBetResponse = await AsiaGamingApiClient.SettleBetAsync(
                    loginRecordId, playerCode, externalRoundId, 0);

                if (loseBetResponse == null)
                {
                    Logger.Warn($"Player {userId} failed to lose a bet");
                    return;
                }

                Logger.Info($"Player {userId} lost a bet");
            }
        }

        async ValueTask IGameProviderApi.PlaceSingleBet(string userId,
            CreatePlayerAndLoginResponse createPlayerAndLoginResponse, int count)
        {
            var playerCode = createPlayerAndLoginResponse.PlayerCode;
            var loginRecordId = createPlayerAndLoginResponse.LoginRecordId;

            var externalRoundId = $"{userId}-Place-{DateTime.Now:yyMMddHHmmss}-{count}";
            var placeBetResponse =
                await AsiaGamingApiClient.PlaceBetAsync(loginRecordId, playerCode, externalRoundId);

            if (placeBetResponse == null || placeBetResponse.ErrorCode != AsiaGamingSettings.SuccessfulErrorCode)
            {
                Logger.Warn($"Player {userId} failed to place a bet");
                await Task.Delay(1000);
                return;
            }

            Logger.Info($"Player {userId} placed a bet");

            if (count % 5 == 0)
            {
                Logger.Info($"Player {userId} settling a bet");
                var settleBetResponse = await AsiaGamingApiClient.SettleBetAsync(
                    loginRecordId, playerCode, externalRoundId, 10);

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
                var loseBetResponse = await AsiaGamingApiClient.SettleBetAsync(
                    loginRecordId, playerCode, externalRoundId, 0);

                if (loseBetResponse == null)
                {
                    Logger.Warn($"Player {userId} failed to lose a bet");
                    return;
                }

                Logger.Info($"Player {userId} lost a bet");
            }
        }

        ValueTask IGameProviderApi.PlaceOppositeBets(string userId,
            CreatePlayerAndLoginResponse createPlayerAndLoginResponse, int count) =>
            throw new NotImplementedException();

        ValueTask IGameProviderApi.PlaceDuplicateSingleBet(string userId,
            CreatePlayerAndLoginResponse createPlayerAndLoginResponse, int count) =>
            throw new NotImplementedException();
    }
}