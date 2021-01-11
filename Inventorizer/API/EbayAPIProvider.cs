using System;
using System.Net.Http;
using System.Net.Http.Json;

using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.WebUtilities;

using Inventorizer.API.Auth;

namespace Inventorizer.API
{
    public class EbayAPIProvider
    {
        private readonly IConfiguration _configuration;

        private readonly IHttpClientFactory _clientFactory;

        private readonly EbayAPIAuthService _ebayAPIAuthService;

        /*
        Unchanging request parameters that are applied to all items
        Since API expects strings for all params, we're using strings for numerical values
        */
        private readonly Dictionary<string, string> _baseRequestParams = new Dictionary<string, string>
        {
            { "itemLocationCountry", "NL" },
            { "priceCurrency", "EUR" },
            { "conditions", "USED" },
            { "offset", "0" },
            { "limit", "10" }
        };

        public string ErrorString { get; private set; }

        public EbayAPIProvider(IConfiguration configuration, IHttpClientFactory clientFactory, EbayAPIAuthService ebayAPIAuthService)
        {
            _configuration = configuration;
            _clientFactory = clientFactory;

            _ebayAPIAuthService = ebayAPIAuthService;
        }

        public void Test()
        {
            string requestURL = QueryHelpers.AddQueryString(_configuration["EbayAPI:Base"], _baseRequestParams);

            Console.WriteLine(requestURL);
        }
    }
}