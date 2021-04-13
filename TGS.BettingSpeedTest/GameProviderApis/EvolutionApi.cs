using System;
using System.Threading.Tasks;
using NLog;
using TGS.BettingSpeedTest.Clients;
using TGS.BettingSpeedTest.Messages;
using TGS.BettingSpeedTest.Messages.Operator;
using TGS.BettingSpeedTest.Settings;

namespace TGS.BettingSpeedTest.GameProviderApis
{
    public class EvolutionApi : IGameProviderApi
    {
        private static readonly IEvolutionApiClient EvolutionApiClient = new EvolutionApiClient();
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        string IGameProviderApi.SetupHttpsForEndpoint(bool enableHttps)
        {
            EvolutionSettings.ApiEndpoint =
                string.Format(EvolutionSettings.ApiEndpoint, enableHttps ? "https" : "http");
            return EvolutionSettings.ApiEndpoint;
        }

        Task IGameProviderApi.InitializeAsync(string userId, string playerToken, Guid? rewardCampaignId)
        {
            return Task.FromResult(0);
        }

        Task<GetAccessTokenResponse> IGameProviderApi.GetGameProviderAccessTokenResponseAsync()
        {
            return Task.FromResult(new GetAccessTokenResponse());
        }

        Task IGameProviderApi.PlaceSingleBet(string userId, PlayerDataResponse playerData, Guid? rewardCampaignId,
            TokenStore oauthToken, AuthorizePlayerResponse authorizePlayerResponse, bool enableJackpot, int count)
        {
            return DebitThenPlaceAndSettleBetAsync(userId, playerData, authorizePlayerResponse, count);
        }

        Task IGameProviderApi.PlaceMultipleBets(string userId, PlayerDataResponse playerData,
            Guid? rewardCampaignId, TokenStore oauthToken, AuthorizePlayerResponse authorizePlayerResponse,
            bool enableJackpot, int count)
        {
            return DebitThenPlaceAndSettleBetAsync(userId, playerData, authorizePlayerResponse, count);
        }

        Task IGameProviderApi.PlaceOppositeBets(string userId, PlayerDataResponse playerData, Guid? rewardCampaignId,
            TokenStore oauthToken, AuthorizePlayerResponse authorizePlayerResponse, bool enableJackpot, int count)
        {
            return DebitThenPlaceAndSettleBetAsync(userId, playerData, authorizePlayerResponse, count);
        }

        Task IGameProviderApi.PlaceDuplicateSingleBet(string userId, PlayerDataResponse playerData,
            Guid? rewardCampaignId, TokenStore oauthToken, AuthorizePlayerResponse authorizePlayerResponse,
            bool enableJackpot, int count)
        {
            throw new NotImplementedException();
        }

        private async Task DebitThenPlaceAndSettleBetAsync(string userId, PlayerDataResponse playerData, 
            AuthorizePlayerResponse authorizePlayerResponse, int count)
        {
            var externalRoundId = $"{userId}-Place-{DateTime.Now:yyMMddHHmmss}-{count}";
            var debitResopnse = await EvolutionApiClient.DebitAsync(authorizePlayerResponse.authtoken, playerData.playerid, externalRoundId);

            if (debitResopnse == null || debitResopnse.Status != EvolutionSettings.SuccessfulErrorCode)
            {
                Logger.Warn($"Player {userId} failed to debit");
                await Task.Delay(1000);
                return;
            }
            Logger.Info($"Player {userId} debit");

            if (count % 5 == 0)
            {
                Logger.Info($"Player {userId} placing and settling a bet");
                
                var settleBetResopnse = await EvolutionApiClient.PlaceAndSettleBetAsync(
                    authorizePlayerResponse.authtoken, playerData.playerid, externalRoundId, 10);

                if (settleBetResopnse == null)
                {
                    Logger.Warn($"Player {userId} failed to place and settle a bet");
                    return;
                }

                Logger.Info($"Player {userId} placed and settled a bet");
            }
            else
            {
                Logger.Info($"Player {userId} placing and settling a lose bet");
                
                var loseBetResponse = await EvolutionApiClient.PlaceAndSettleBetAsync(
                    authorizePlayerResponse.authtoken, playerData.playerid, externalRoundId, 0);

                if (loseBetResponse == null)
                {
                    Logger.Warn($"Player {userId} failed to place and settle a lose bet");
                    return;
                }

                Logger.Info($"Player {userId} placed and settled a lose bet");
            }
        }
    }
}