using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using NLog;

namespace TGS.BettingSpeedTest.Clients
{
    internal abstract class BaseApiClient
    {
        private const int TimeoutInSeconds = 15;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private static readonly HttpClient HttpClient = new();

        protected static async ValueTask<TResponse> SendAndReadFromJsonAsync<TResponse>(HttpRequestMessage request,
            int timeoutSeconds = TimeoutInSeconds, JsonSerializerOptions jsonSerializerOptions = null)
        {
            try
            {
                using var response = await HttpClient.SendAsync(request,
                    new CancellationTokenSource(TimeSpan.FromSeconds(timeoutSeconds)).Token);

                var value = await response.Content.ReadAsStringAsync();
                var o = JsonSerializer.Deserialize<TResponse>(value, jsonSerializerOptions);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    return o;
                }

                Logger.Warn(value);
                return o;
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                return default;
            }
        }

        protected static async ValueTask<TResponse> SendAndReadFromXmlAsync<TResponse>(HttpRequestMessage request,
            int timeoutSeconds = TimeoutInSeconds)
        {
            try
            {
                using var response = await HttpClient.SendAsync(request,
                    new CancellationTokenSource(TimeSpan.FromSeconds(timeoutSeconds)).Token);

                var value = await response.Content.ReadAsStringAsync();

                using var sr = new StringReader(value);
                var serializer = new XmlSerializer(typeof(TResponse));
                var o = (TResponse) serializer.Deserialize(sr);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    return o;
                }

                Logger.Warn(value);
                return o;
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                return default;
            }
        }
    }
}