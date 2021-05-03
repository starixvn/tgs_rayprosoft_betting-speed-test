using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TGS.BettingSpeedTest.Messages.Global;
using TGS.BettingSpeedTest.Settings;

namespace TGS.BettingSpeedTest.Clients
{
    public interface IGlobalApiClient
    {
        ValueTask<CreatePlayerAndLoginResponse> CreatePlayerAndLoginAsync(CreatePlayerAndLoginRequest request);
    }

    internal class GlobalApiClient : BaseApiClient, IGlobalApiClient
    {
        private readonly JsonSerializerOptions
            _jsonSerializerOptions = new() {WriteIndented = false};

        ValueTask<CreatePlayerAndLoginResponse> IGlobalApiClient.CreatePlayerAndLoginAsync(
            CreatePlayerAndLoginRequest request)
        {
            var url = GlobalApiSettings.ApiEndpoint + GlobalApiSettings.ApiPath.CreatePlayerAndLogin;

            var value = JsonSerializer.Serialize(request, _jsonSerializerOptions);
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new StringContent(value, Encoding.UTF8, "application/json")
            };

            return SendAndReadFromJsonAsync<CreatePlayerAndLoginResponse>(httpRequestMessage,
                jsonSerializerOptions: _jsonSerializerOptions);
        }
    }
}