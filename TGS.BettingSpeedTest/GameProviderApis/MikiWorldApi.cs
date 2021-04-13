using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NLog;
using TGS.BettingSpeedTest.Clients;
using TGS.BettingSpeedTest.Messages;
using TGS.BettingSpeedTest.Messages.MikiWorld;
using TGS.BettingSpeedTest.Messages.Operator;
using TGS.BettingSpeedTest.Settings;

namespace TGS.BettingSpeedTest.GameProviderApis
{
    public class MikiWorldApi : IGameProviderApi
    {
        private static readonly IMikiWorldApiClient MikiWorldApiClient = new MikiWorldApiClient();
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        string IGameProviderApi.SetupHttpsForEndpoint(bool enableHttps)
        {
            MikiWorldSettings.ApiEndpoint =
                string.Format(MikiWorldSettings.ApiEndpoint, enableHttps ? "https" : "http");
            return MikiWorldSettings.ApiEndpoint;
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
            var placeBetResopnse = await MikiWorldApiClient.PlaceBetAsync(authorizePlayerResponse.authtoken, playerData.playercode, externalRoundId);

            if (placeBetResopnse == null || placeBetResopnse.Status != MikiWorldSettings.SuccessfulErrorCode)
            {
                Logger.Warn($"Player {userId} failed to place a bet");
                await Task.Delay(1000);
                return;
            }
            Logger.Info($"Player {userId} placed a bet");

            if (count % 5 == 0)
            {
                Logger.Info($"Player {userId} settling a bet");
                var settleBetResopnse = await MikiWorldApiClient.SettleBetAsync(
                    authorizePlayerResponse.authtoken, playerData.playercode, externalRoundId, 10);

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
                var loseBetResponse = await MikiWorldApiClient.SettleBetAsync(
                    authorizePlayerResponse.authtoken, playerData.playercode, externalRoundId, 0);

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
            var requests = new List<Task<TransactionResponse>>();
            requests.AddRange(Enumerable.Range(0, 3).Select(r => MikiWorldApiClient.PlaceBetAsync(
                authorizePlayerResponse.authtoken, playerData.playercode, externalRoundId)));

            var responses = await Task.WhenAll(requests);

            if (responses.First().Status != MikiWorldSettings.SuccessfulErrorCode)
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
                var settleBetResopnse = await MikiWorldApiClient.SettleBetAsync(
                    authorizePlayerResponse.authtoken, playerData.playercode, externalRoundId, 10);

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
                var loseBetResponse = await MikiWorldApiClient.SettleBetAsync(
                    authorizePlayerResponse.authtoken, playerData.playercode, externalRoundId, 0);

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
            var requests = new List<Task<TransactionResponse>>();
            requests.AddRange(Enumerable.Range(0, 3).Select(r => MikiWorldApiClient.PlaceBetAsync(
                authorizePlayerResponse.authtoken, playerData.playercode, externalRoundId)));

            var responses = await Task.WhenAll(requests);

            if (responses.First().Status != MikiWorldSettings.SuccessfulErrorCode)
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
                var settleBetResopnse = await MikiWorldApiClient.SettleBetAsync(
                    authorizePlayerResponse.authtoken, playerData.playercode, externalRoundId, 10);

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
                var loseBetResponse = await MikiWorldApiClient.SettleBetAsync(
                    authorizePlayerResponse.authtoken, playerData.playercode, externalRoundId, 0);

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