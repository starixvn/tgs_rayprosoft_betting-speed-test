using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TGS.BettingSpeedTest.Messages.AWC;

namespace TGS.BettingSpeedTest.Clients
{
    internal interface IAWCApiClient
    {
        Task<Response>RequestAsync(string url, string key, string playerToken, Transaction[] transactions, string action = "bet");
    }

    internal sealed class AWCApiClient : BaseApiClient, IAWCApiClient
    {
        Task<Response> IAWCApiClient.RequestAsync(string url, string key, string playerToken, Transaction[] transactions, string action)
        {
            var message = new JObject
            {
                ["action"] = action,
                ["txns"] = JArray.FromObject(transactions)
            };

            var transactionRequest = new TransactionRequest
            {
                Key = key,
                Extension = JsonConvert.SerializeObject(new JObject {["token"] = playerToken}),
                Message = JsonConvert.SerializeObject(message)
            };

            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new FormUrlEncodedContent(ConvertToKeyValue(transactionRequest))
            };

            return SendAsync<Response>(request);
        }
    }
}
