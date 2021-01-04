using System.Net.Http;
using System.Collections.Generic;

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
        private string _applicationAccessToken;

        private IHttpClientFactory _clientFactory;

        private IConfiguration _configuration;

        public EbayAPI(IConfiguration configuration)
        {
            _configuration = configuration;

            string clientId = _configuration["ClientId"];
            string clientSecret = _configuration["ClientSecret"];

            /*
            Retrieve access token
            */
        }

        public List<double> RetrieveItemPrices()
        {
            return new List<double>();
        }

        private void RetrieveApplicationAccessToken()
        {

        }
    }
}