using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using NLog;
using TGS.BettingSpeedTest.Extensions;

namespace TGS.BettingSpeedTest.Clients
{
    internal abstract class BaseApiClient
    {
        private const int TimeoutInSeconds = 15;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private static readonly HttpClient HttpClient = new HttpClient();
        private const string DefaultXmlMediaType = "text/xml";
        private static readonly XmlMediaTypeFormatter DefaultXmlMediaTypeFormatter = new XmlMediaTypeFormatter { UseXmlSerializer = true };

        protected async Task<TResponse> SendAsync<TResponse>(HttpRequestMessage request, int timeoutSeconds = TimeoutInSeconds)
        {
            try
            {
                using (var response = await HttpClient.SendAsync(request, new CancellationTokenSource(TimeSpan.FromSeconds(timeoutSeconds)).Token))
                {
                    var content = await response.Content.ReadAsAsync<TResponse>();

                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        var error = await response.Content.ReadAsStringAsync();
                        Logger.Warn(error);
                        return content;
                    }

                    return content;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                return default;
            }
        }

        protected async Task<TResponse> SendXmlAsync<TRequest, TResponse>(TRequest body, string url, int timeoutSeconds = TimeoutInSeconds)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Post, url)
                {
                    Content = new ObjectContent<TRequest>(body, DefaultXmlMediaTypeFormatter, DefaultXmlMediaType)
                };

                using (var response = await HttpClient.SendAsync(request, new CancellationTokenSource(TimeSpan.FromSeconds(timeoutSeconds)).Token))
                {
                    var content = await response.Content.ReadAsAsync<TResponse>(new[] { DefaultXmlMediaTypeFormatter });

                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        var error = await response.Content.ReadAsStringAsync();
                        Logger.Warn(error);
                        return content;
                    }

                    return content;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                return default;
            }
        }

        protected static IEnumerable<KeyValuePair<string, string>> ConvertToKeyValue(object metaToken)
        {
            if (metaToken == null)
            {
                return null;
            }

            if (!(metaToken is JToken token))
            {
                return ConvertToKeyValue(JObject.FromObject(metaToken));
            }

            if (token.HasValues)
            {
                var contentData = new List<KeyValuePair<string, string>>();
                foreach (var child in token.Children())
                {
                    var childContent = ConvertToKeyValue(child);
                    if (childContent != null)
                    {
                        contentData.AddRange(childContent);
                    }
                }

                return contentData;
            }

            var jValue = token as JValue;
            if (jValue?.Value == null)
            {
                //can't omit if token was a element in the array
                if (token.Path.Contains("]"))
                    return new[] { new KeyValuePair<string, string>(token.Path, null) };

                return null;
            }

            var value = jValue.Type == JTokenType.Date ?
                jValue.ToString("o", CultureInfo.InvariantCulture) :
                jValue.ToString(CultureInfo.InvariantCulture);

            return new[] { new KeyValuePair<string, string>(token.Path, value) };
        }
    }
}