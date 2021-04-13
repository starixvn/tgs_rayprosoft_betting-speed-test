using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using TGS.BettingSpeedTest.Extensions;
using TGS.BettingSpeedTest.Messages;
using TGS.BettingSpeedTest.Messages.Operator;
using TGS.BettingSpeedTest.Messages.SunbetCredit;
using TGS.BettingSpeedTest.Settings;

namespace TGS.BettingSpeedTest.Clients
{
    public interface IOperatorApiClient
    {
        Task<GetAccessTokenResponse> GetAccessTokenAsync(CredentialAndLobbyPath credential);

        Task<AuthorizePlayerResponse> AuthorizePlayerAsync(string accessToken, string userId);

        Task<CreditResponse> CreditAsync(string accessToken, string userId, string externalTransactionId);

        Task<PlayerDataResponse> PlayerInfoAsync(string playerToken);

        Task<ApiResponse<AuthenticateManyResponse[]>> SunbetCreditAuthManyAsync(int playerIndex);

        Task<GetSunbetCreditLobbyTokenResponse> SunbetCreditTokenAuthedAsync(string sessionId, string acctId, int siteId);

        Task<CreditSunbetCreditLobbyWalletResponse> SunbetCreditWalletCreditAsync(string accessToken, Guid playerId);

        Task<DebitSunbetCreditLobbyWalletResponse> SunbetCreditWalletDebitAsync(string accessToken, Guid playerId);

        Task<ApiResponse<GetPlayerInfoResponse>> SunbetCreditGetPlayerInfoAsync(string playerId);
    }

    internal class OperatorApiClient : BaseApiClient, IOperatorApiClient
    {
        Task<GetAccessTokenResponse> IOperatorApiClient.GetAccessTokenAsync(CredentialAndLobbyPath credential)
        {
            var url = OperatorSettings.ApiEndpoint + OperatorSettings.ApiPath.OAuthToken;

            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new FormUrlEncodedContent(new[] {
                    new KeyValuePair<string, string>("client_id", credential.ClientId),
                    new KeyValuePair<string, string>("client_secret", credential.ClientSecret),
                    new KeyValuePair<string, string>("grant_type", OperatorSettings.OAuth.GrantType),
                    new KeyValuePair<string, string>("scope", OperatorSettings.OAuth.Scope) })
            };

            return SendAsync<GetAccessTokenResponse>(request);
        }

        Task<AuthorizePlayerResponse> IOperatorApiClient.AuthorizePlayerAsync(string accessToken, string userId)
        {
            var url = OperatorSettings.ApiEndpoint + OperatorSettings.ApiPath.PlayerAuthorize;

            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new ObjectContent<AuthorizePlayerRequest>(new AuthorizePlayerRequest
                {
                    ipaddress = OperatorSettings.Player.IpAddress,
                    username = userId,
                    userid = userId,
                    lang = OperatorSettings.Player.Language,
                    cur = OperatorSettings.Player.Currency,
                    betlimitid = OperatorSettings.Player.BetlimitId,
                    istestplayer = OperatorSettings.Player.IsTestPlayer,
                    platformtype = 0
                }, new JsonMediaTypeFormatter())
            };

            request.Headers.Authorization = new AuthenticationHeaderValue(OperatorSettings.OAuth.TokenType, accessToken);

            return SendAsync<AuthorizePlayerResponse>(request);
        }

        Task<CreditResponse> IOperatorApiClient.CreditAsync(string accessToken, string userId, string externalTransactionId)
        {
            var url = OperatorSettings.ApiEndpoint + OperatorSettings.ApiPath.WalletCredit;

            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new ObjectContent<CreditRequest>(new CreditRequest
                {
                    userid = userId,
                    amt = OperatorSettings.Wallet.CreditAmount,
                    cur = OperatorSettings.Player.Currency,
                    txid = externalTransactionId,
                    timestamp = DateTimeOffset.Now.ToIso8601String()
                }, new JsonMediaTypeFormatter())
            };

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            return SendAsync<CreditResponse>(request);
        }

        async Task<PlayerDataResponse> IOperatorApiClient.PlayerInfoAsync(string playerToken)
        {
            var url = OperatorSettings.SystemApiEndpoint + OperatorSettings.ApiPath.PlayerInfo + playerToken;

            var request = new HttpRequestMessage(HttpMethod.Get, url)
            {
            };

            var res = await SendAsync<PlayerDataResponse>(request, 30);

            return res;
        }

        async Task<ApiResponse<AuthenticateManyResponse[]>> IOperatorApiClient.SunbetCreditAuthManyAsync(int playerIndex)
        {
            var url = OperatorSettings.LobbyApiEndpoint + OperatorSettings.ApiPath.SunbetCreditAuthMany;

            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new ObjectContent<AuthenticateManyRequest>(new AuthenticateManyRequest
                {
                    clientIp = OperatorSettings.Player.IpAddress,
                    player_id_from = playerIndex,
                    player_id_to = playerIndex
                }, new JsonMediaTypeFormatter())
            };

            var res = await SendAsync<ApiResponse<AuthenticateManyResponse[]>>(request, 30);

            return res;
        }

        async Task<GetSunbetCreditLobbyTokenResponse> IOperatorApiClient.SunbetCreditTokenAuthedAsync(string sessionId, string acctId, int siteId)
        {
            var url = OperatorSettings.LobbyApiEndpoint + OperatorSettings.ApiPath.SunbetCreditTokenAuthed;

            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new ObjectContent<GetTokenAuthedRequest>(new GetTokenAuthedRequest
                {
                    AcctLogin = sessionId,
                    AcctId = acctId,
                    SiteId = siteId,
                    SessionId = sessionId,
                    Lang = OperatorSettings.Player.Language,
                    ClientIpAddress = OperatorSettings.Player.IpAddress,
                    PlatformType = 0
                }, new JsonMediaTypeFormatter())
            };

            var res = await SendAsync<GetSunbetCreditLobbyTokenResponse>(request, 30);

            return res;
        }

        async Task<CreditSunbetCreditLobbyWalletResponse> IOperatorApiClient.SunbetCreditWalletCreditAsync(string accessToken, Guid playerId)
        {
            var url = OperatorSettings.LobbyApiEndpoint + OperatorSettings.ApiPath.SunbetCreditWalletCredit;

            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new ObjectContent<CreditSunbetCreditLobbyWalletRequest>(new CreditSunbetCreditLobbyWalletRequest
                {
                    SunbetCreditToken = accessToken,
                    Amount = OperatorSettings.Wallet.CreditAmount,
                    PlayerId = playerId
                }, new JsonMediaTypeFormatter())
            };

            var res = await SendAsync<CreditSunbetCreditLobbyWalletResponse>(request, 30);

            return res;
        }

        async Task<DebitSunbetCreditLobbyWalletResponse> IOperatorApiClient.SunbetCreditWalletDebitAsync(string accessToken, Guid playerId)
        {
            var url = OperatorSettings.LobbyApiEndpoint + OperatorSettings.ApiPath.SunbetCreditWalletDebit;

            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new ObjectContent<DebitPlayerWalletRequest>(new DebitPlayerWalletRequest
                {
                    SunbetCreditToken = accessToken,
                    PlayerId = playerId
                }, new JsonMediaTypeFormatter())
            };

            var res = await SendAsync<DebitSunbetCreditLobbyWalletResponse>(request, 30);

            return res;
        }

        async Task<ApiResponse<GetPlayerInfoResponse>> IOperatorApiClient.SunbetCreditGetPlayerInfoAsync(string playerId)
        {
            var url = OperatorSettings.SunbetCreditApiEndpoint + OperatorSettings.ApiPath.SunbetCreditPlayerInfo;

            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new ObjectContent<GetPlayerInfoRequest>(new GetPlayerInfoRequest
                {
                    player_id = playerId
                }, new JsonMediaTypeFormatter())
            };

            GenerateAuthorizationHeader(request);
            var res = await SendAsync<ApiResponse<GetPlayerInfoResponse>>(request, 30);

            return res;
        }

        private void GenerateAuthorizationHeader(HttpRequestMessage req)
        {
            var scmDate = DateTime.UtcNow.ToString("yyyy-MM-ddThh:mm:ss.fffZ");
            var signature = GetSignature(OperatorSettings.OAuth.SunbetCreditDeskTopClientSecret, scmDate);
            var authorization = $"TGP {OperatorSettings.OAuth.SunbetCreditDeskTopClientId}:{signature}";
            req.Headers.Add("Authorization", authorization);
            req.Headers.Add("X-TGP-Date", scmDate);
        }

        private string GetSignature(string appSecret, string scmDate)
        {
            var stringToSign = appSecret + scmDate;
            var keys = Encoding.ASCII.GetBytes(appSecret);
            var bytes = Encoding.UTF8.GetBytes(stringToSign);
            var hmac = new HMACSHA1(keys);
            var hashed = hmac.ComputeHash(bytes);
            var signature = Convert.ToBase64String(hashed);
            return signature;
        }
    }
}