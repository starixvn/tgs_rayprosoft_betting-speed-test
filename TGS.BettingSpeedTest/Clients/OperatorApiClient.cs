using System;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using TGS.BettingSpeedTest.Extensions;
using TGS.BettingSpeedTest.Messages.Operator;
using TGS.BettingSpeedTest.Settings;

namespace TGS.BettingSpeedTest.Clients
{
    public interface IOperatorApiClient
    {
        Task<AuthorizePlayerResponse> AuthorizePlayerAsync(string accessToken, string userId);

        Task<CreditResponse> CreditAsync(string accessToken, string userId, string externalTransactionId);

        Task<PlayerDataResponse> PlayerInfoAsync(string playerToken);
    }

    internal class OperatorApiClient : BaseApiClient, IOperatorApiClient
    {
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

            //request.Headers.Authorization =
            //    new AuthenticationHeaderValue(OperatorSettings.OAuth.TokenType, accessToken);

            return SendAsync<AuthorizePlayerResponse>(request);
        }

        Task<CreditResponse> IOperatorApiClient.CreditAsync(string accessToken, string userId,
            string externalTransactionId)
        {
            var url = OperatorSettings.ApiEndpoint + OperatorSettings.ApiPath.WalletCredit;

            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new ObjectContent<CreditRequest>(new CreditRequest
                {
                    userid = userId,
                    amt = OperatorSettings.Wallet.InitAmount,
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

            var request = new HttpRequestMessage(HttpMethod.Get, url);

            var res = await SendAsync<PlayerDataResponse>(request, 30);

            return res;
        }


        //private void GenerateAuthorizationHeader(HttpRequestMessage req)
        //{
        //    var scmDate = DateTime.UtcNow.ToString("yyyy-MM-ddThh:mm:ss.fffZ");
        //    var signature = GetSignature(OperatorSettings.OAuth.SunbetCreditDeskTopClientSecret, scmDate);
        //    var authorization = $"TGP {OperatorSettings.OAuth.SunbetCreditDeskTopClientId}:{signature}";
        //    req.Headers.Add("Authorization", authorization);
        //    req.Headers.Add("X-TGP-Date", scmDate);
        //}

        //private string GetSignature(string appSecret, string scmDate)
        //{
        //    var stringToSign = appSecret + scmDate;
        //    var keys = Encoding.ASCII.GetBytes(appSecret);
        //    var bytes = Encoding.UTF8.GetBytes(stringToSign);
        //    var hmac = new HMACSHA1(keys);
        //    var hashed = hmac.ComputeHash(bytes);
        //    var signature = Convert.ToBase64String(hashed);
        //    return signature;
        //}
    }
}