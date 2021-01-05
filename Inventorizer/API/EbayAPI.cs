using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.Extensions.Configuration;

/*
@TODO:

1. Add ebay's auth and access urls to appsettings
2. Add client_id and client_secret to userSecrets
3. Retrieve OAuth token from auth url
4. Test by retrieving simple list
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
            // Using auth URL explicitly, since httpClient is geared towards ebay API base address
            HttpRequestMessage requestToAuth = new HttpRequestMessage(
                HttpMethod.Post,
                _configuration["EbayAPI:Auth"]
            );

            HttpClient client = _clientFactory.CreateClient("EbayAPI");

            HttpResponseMessage responseFromAuth = await client.SendAsync(requestToAuth);

            if (responseFromAuth.IsSuccessStatusCode)
            {

            }
        }
    }
}