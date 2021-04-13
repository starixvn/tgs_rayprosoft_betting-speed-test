using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NLog;
using TGS.BettingSpeedTest.Clients;
using TGS.BettingSpeedTest.Messages;
using TGS.BettingSpeedTest.Messages.Operator;
using TGS.BettingSpeedTest.Messages.XPG;
using TGS.BettingSpeedTest.Settings;

namespace TGS.BettingSpeedTest.GameProviderApis
{
    public class XPGApi : IGameProviderApi
    {
        private static readonly IXPGApiClient XPGApiClient = new XPGApiClient();
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        string IGameProviderApi.SetupHttpsForEndpoint(bool enableHttps)
        {
            XPGSettings.ApiEndpoint =
                string.Format(XPGSettings.ApiEndpoint, enableHttps ? "https" : "http");
            return XPGSettings.ApiEndpoint;
        }

        Task IGameProviderApi.InitializeAsync(string userId, string playerToken, Guid? rewardCampaignId)
        {
            return Task.FromResult(0);
        }

        Task<GetAccessTokenResponse> IGameProviderApi.GetGameProviderAccessTokenResponseAsync()
        {
            return Task.FromResult(new GetAccessTokenResponse());
        }

        async Task IGameProviderApi.PlaceSingleBet(string userId, PlayerDataResponse playerData, Guid? rewardCampaignId,
            TokenStore oauthToken, AuthorizePlayerResponse authorizePlayerResponse, bool enableJackpot, int count)
        {
            var externalRoundId = $"{userId}-Place-{DateTime.Now:yyMMddHHmmss}-{count}";
            var placeBetResopnse = await XPGApiClient.PlaceBetAsync(authorizePlayerResponse.authtoken, playerData.playercode, externalRoundId);

            if (placeBetResopnse == null || placeBetResopnse.Data.ErrorCode != 0)
            {
                Logger.Warn($"Player {userId} failed to place a bet");
                await Task.Delay(1000);
                return;
            }
            Logger.Info($"Player {userId} placed a bet");

            if (count % 5 == 0)
            {
                Logger.Info($"Player {userId} settling a bet");
                var settleBetResopnse = await XPGApiClient.SettleBetAsync(
                    authorizePlayerResponse.authtoken, playerData.playercode, externalRoundId, 10, false);

                if (settleBetResopnse == null)
                {
                    Logger.Warn($"Player {userId} failed to settle a bet");
                    return;
                }

                Logger.Info($"Player {userId} settled a bet");
            }
            else
            {
                Logger.Info($"Player {userId} losing a bet");
                var loseBetResponse = await XPGApiClient.SettleBetAsync(
                    authorizePlayerResponse.authtoken, playerData.playercode, externalRoundId, 0, false);

                if (loseBetResponse == null)
                {
                    Logger.Warn($"Player {userId} failed to lose a bet");
                    return;
                }

                Logger.Info($"Player {userId} lost a bet");
            }
        }

        async Task IGameProviderApi.PlaceMultipleBets(string userId, PlayerDataResponse playerData,
            Guid? rewardCampaignId, TokenStore oauthToken, AuthorizePlayerResponse authorizePlayerResponse,
            bool enableJackpot, int count)
        {
            var externalRoundId = $"{userId}-Place-{DateTime.Now:yyMMddHHmmss}-{count}";
            var requests = new List<Task<SuccessResponse>>();
            requests.AddRange(Enumerable.Range(0, 3).Select(r => XPGApiClient.PlaceBetAsync(
                authorizePlayerResponse.authtoken, playerData.playercode, externalRoundId, r)));

            var responses = await Task.WhenAll(requests);

            if (responses.First().Data.ErrorCode != 0)
            {
                Logger.Warn($"Player {userId} failed to place a bet");
                await Task.Delay(1000);
                return;
            }

            Logger.Info($"Player {userId} placed a bet");

            if (count % 5 == 0)
            {
                Logger.Info($"Player {userId} settling a bet");
                string settleBetExternalTransactionId =
                    $"{userId}-Settle-{DateTime.Now:yyyyMMddHHmmss}-{count}";
                var settleBetResopnse = await XPGApiClient.SettleBetAsync(
                    authorizePlayerResponse.authtoken, playerData.playercode, externalRoundId, 10, true);

                if (settleBetResopnse == null)
                {
                    Logger.Warn($"Player {userId} failed to settle a bet");
                    return;
                }

                Logger.Info($"Player {userId} settled a bet");
            }
            else
            {
                Logger.Info($"Player {userId} losing a bet");
                string loseBetExternalTransactionId =
                    $"{userId}-Lose-{DateTime.Now:yyyyMMddHHmmss}-{count}";
                var loseBetResponse = await XPGApiClient.SettleBetAsync(
                    authorizePlayerResponse.authtoken, playerData.playercode, externalRoundId, 0, true);

                if (loseBetResponse == null)
                {
                    Logger.Warn($"Player {userId} failed to lose a bet");
                    return;
                }

                Logger.Info($"Player {userId} lost a bet");
            }
        }

        async Task IGameProviderApi.PlaceOppositeBets(string userId, PlayerDataResponse playerData, Guid? rewardCampaignId,
            TokenStore oauthToken, AuthorizePlayerResponse authorizePlayerResponse, bool enableJackpot, int count)
        {
            var externalRoundId = $"{userId}-Place-{DateTime.Now:yyMMddHHmmss}-{count}";
            var requests = new List<Task<SuccessResponse>>();
            requests.AddRange(Enumerable.Range(0, 3).Select(r => XPGApiClient.PlaceBetAsync(
                authorizePlayerResponse.authtoken, playerData.playercode, externalRoundId, r)));

            var responses = await Task.WhenAll(requests);

            if (responses.First().Data.ErrorCode != 0)
            {
                Logger.Warn($"Player {userId} failed to place a bet");
                await Task.Delay(1000);
                return;
            }

            Logger.Info($"Player {userId} placed a bet");

            if (count % 5 == 0)
            {
                Logger.Info($"Player {userId} settling a bet");
                string settleBetExternalTransactionId =
                    $"{userId}-Settle-{DateTime.Now:yyyyMMddHHmmss}-{count}";
                var settleBetResopnse = await XPGApiClient.SettleBetAsync(
                    authorizePlayerResponse.authtoken, playerData.playercode, externalRoundId, 10, true);

                if (settleBetResopnse == null)
                {
                    Logger.Warn($"Player {userId} failed to settle a bet");
                    return;
                }

                Logger.Info($"Player {userId} settled a bet");
            }
            else
            {
                Logger.Info($"Player {userId} losing a bet");
                string loseBetExternalTransactionId =
                    $"{userId}-Lose-{DateTime.Now:yyyyMMddHHmmss}-{count}";
                var loseBetResponse = await XPGApiClient.SettleBetAsync(
                    authorizePlayerResponse.authtoken, playerData.playercode, externalRoundId, 0, true);

                if (loseBetResponse == null)
                {
                    Logger.Warn($"Player {userId} failed to lose a bet");
                    return;
                }

                Logger.Info($"Player {userId} lost a bet");
            }
        }

        Task IGameProviderApi.PlaceDuplicateSingleBet(string userId, PlayerDataResponse playerData,
            Guid? rewardCampaignId, TokenStore oauthToken, AuthorizePlayerResponse authorizePlayerResponse,
            bool enableJackpot, int count)
        {
            throw new NotImplementedException();
        }
    }
}