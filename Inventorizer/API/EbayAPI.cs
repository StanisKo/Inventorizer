using System;
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
        private IConfiguration _configuration;

        private IHttpClientFactory _clientFactory;

        private string _clientId;
        private string _clientSecret;
        private string _applicationAccessToken;

        public EbayAPI(IConfiguration configuration)
        {
            _configuration = configuration;

            _clientId = _configuration["ClientId"];
            _clientSecret = _configuration["ClientSecret"];
        }

        public async Task<List<double>> RetrieveItemPrices(List<string> itemNames)
        {
            return new List<double>();
        }

        private async Task RetrieveApplicationAccessToken()
        {

        }
    }
}