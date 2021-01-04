using System.Net.Http;
using System.Collections.Generic;

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
        private IHttpClientFactory ClientFactory { get; }

        private readonly string ApplicationToken { get; }

        public EbayAPI()
        {
            // Retrieve access token here
        }
    }
}