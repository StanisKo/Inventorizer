using System;
using System.Text;
using System.Net.Http;
using System.Net.Http.Json;
using System.Net.Http.Headers;

using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.Extensions.Configuration;

/*
@TODO:

1. Add ebay's auth and access urls to appsettings
2. Add client_id and client_secret to userSecrets
3. Retrieve OAuth token from auth url
4. Test by retrieving simple list

5. Move api to separate class library, add as singletone and initialize on acessing home
*/

namespace Inventorizer.API
{
    public class EbayAPI
    {
        private readonly IConfiguration _configuration;

        private readonly IHttpClientFactory _clientFactory;

        private string _clientId;
        private string _clientSecret;
        private string _applicationAccessToken;

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
        }

        public async Task<List<double>> RetrieveItemPrices(List<string> itemNames)
        {
            return new List<double>();
        }

        private async Task RetrieveApplicationAccessToken()
        {
            HttpClient client = _clientFactory.CreateClient("EbayAPI");

            /*
            Encode portal-provided application keys as base64 and add to headers
            in order to authorize authentication
            */
            byte[] byteArray = Encoding.ASCII.GetBytes($"{_clientId}:{_clientSecret}");

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Basic",
                Convert.ToBase64String(byteArray)
            );

            // Specify grant type and scope of available API
            List<KeyValuePair<string, string>> requestParams = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("grant_type", "client_credentials"),
                new KeyValuePair<string, string>("scope", "https://api.ebay.com/oauth/api_scope")
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

            /*    ****    ****    */

            if (responseFromAuth.IsSuccessStatusCode)
            {
                object applicationAccessTokenResponse = responseFromAuth.Content.ReadFromJsonAsync<object>();
            }
        }
    }
}