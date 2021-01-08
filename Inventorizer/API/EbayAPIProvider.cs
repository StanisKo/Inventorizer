using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Net.Http.Headers;

using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.Extensions.Configuration;

namespace Inventorizer.API
{
    public class EbayAPIProvider
    {
        private readonly IConfiguration _configuration;

        private readonly IHttpClientFactory _clientFactory;

        public string ErrorString { get; private set; }

        public EbayAPIProvider(IConfiguration configuration, IHttpClientFactory clientFactory)
        {
            _configuration = configuration;
            _clientFactory = clientFactory;
        }

        // public async Task<List<double>> RetrieveItemPrices(List<string> itemNames)
        // {
        //     return new List<double>();
        // }
    }
}