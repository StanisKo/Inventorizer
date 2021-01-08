using System;
using System.Net.Http;
using System.Net.Http.Json;

using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.Extensions.Configuration;

using Inventorizer.API.Auth;

namespace Inventorizer.API
{
    public class EbayAPIProvider
    {
        private readonly IConfiguration _configuration;

        private readonly IHttpClientFactory _clientFactory;

        private readonly IServiceProvider _serviceProvider;

        private readonly EbayAPIAuthService _ebayAPIAuthService;

        public string ErrorString { get; private set; }

        public EbayAPIProvider(IConfiguration configuration, IHttpClientFactory clientFactory, EbayAPIAuthService ebayAPIAuthService)
        {
            _configuration = configuration;
            _clientFactory = clientFactory;

            _ebayAPIAuthService = ebayAPIAuthService;
        }
    }
}