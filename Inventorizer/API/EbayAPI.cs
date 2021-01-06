using System;
using System.Text;
using System.Net.Http;
using System.Net.Http.Json;
using System.Net.Http.Headers;

using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.Extensions.Configuration;

namespace Inventorizer.API
{
    public class EbayAPI
    {
        private readonly IConfiguration _configuration;

        private readonly IHttpClientFactory _clientFactory;

        private string _clientId;
        private string _clientSecret;

        private ParsedAuth _parsedAuth;

        private bool _applicationTokenIsActive;

        public string ErrorString { get; private set; }

        public EbayAPI(IConfiguration configuration, IHttpClientFactory clientFactory)
        {
            _configuration = configuration;
            _clientFactory = clientFactory;

            _clientId = _configuration["ClientId"];
            _clientSecret = _configuration["ClientSecret"];
        }

        public async Task InitializeAPI()
        {
            await RetrieveApplicationAccessToken();

            // Remint the token after it has expired and avoid auth request while it is active
            await new Task(() => {

            });
        }

        public async Task<List<double>> RetrieveItemPrices(List<string> itemNames)
        {
            return new List<double>();
        }

        private async Task RetrieveApplicationAccessToken()
        {
            HttpClient client = _clientFactory.CreateClient("EbayAPI");

            /*
            Encode portal-provided application keys as base64 string and add to headers
            in order to access application token from OAuth
            */
            byte[] byteArray = Encoding.ASCII.GetBytes($"{_clientId}:{_clientSecret}");

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Basic",
                Convert.ToBase64String(byteArray)
            );

            // Specify grant type and scope of available API
            var requestParams = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("grant_type", _configuration["EbayAPI:GrantType"]),
                new KeyValuePair<string, string>("scope", _configuration["EbayAPI:Scope"])
            };

            HttpRequestMessage requestToAuth = new HttpRequestMessage(
                HttpMethod.Post,
                _configuration["EbayAPI:Auth"]
            );

            // Encode requst params into url and specify required by the API Content-Type header
            requestToAuth.Content = new FormUrlEncodedContent(requestParams);

            requestToAuth.Content.Headers.ContentType = new MediaTypeHeaderValue(
                "application/x-www-form-urlencoded"
            );

            HttpResponseMessage responseFromAuth = await client.SendAsync(requestToAuth);

            if (responseFromAuth.IsSuccessStatusCode)
            {
                _parsedAuth = await responseFromAuth.Content.ReadFromJsonAsync<ParsedAuth>();

                ErrorString = null;
            }
            else
            {
                ErrorString =
                    $"Failed to retrieve token. {(int)responseFromAuth.StatusCode}: {responseFromAuth.ReasonPhrase}";
            }
        }
    }
}