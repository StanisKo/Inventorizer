using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Net.Http.Headers;

using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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

        public EbayAPIProvider(IConfiguration configuration, IHttpClientFactory clientFactory, IServiceProvider serviceProvider)
        {
            _configuration = configuration;
            _clientFactory = clientFactory;
            _serviceProvider = serviceProvider;

            _ebayAPIAuthService = _serviceProvider.GetService<EbayAPIAuthService>();
        }

        public void Test() => Console.WriteLine(_ebayAPIAuthService?.ParsedAuth.access_token ?? "service is null");
    }
}