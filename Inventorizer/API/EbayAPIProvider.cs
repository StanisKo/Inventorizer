using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Net.Http.Headers;

using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.WebUtilities;

using Inventorizer.API.Auth;

namespace Inventorizer.API
{
    public class EbayAPIProvider
    {
        private readonly IConfiguration _configuration;

        private readonly IHttpClientFactory _clientFactory;

        private readonly ILogger<EbayAPIProvider> _logger;

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

        public EbayAPIProvider(IConfiguration configuration,
                               IHttpClientFactory clientFactory,
                               ILogger<EbayAPIProvider> logger,
                               EbayAPIAuthService ebayAPIAuthService)
        {
            _configuration = configuration;
            _clientFactory = clientFactory;
            _logger = logger;

            _ebayAPIAuthService = ebayAPIAuthService;
        }

        /*
        First iteration of provider, things will probably change (due to optimization)
        */
        public async Task<List<double>> RetrieveItemPrices(List<string> itemNames)
        {
            HttpClient client = _clientFactory.CreateClient("EbayAPI");

            // Authenticate call with application access token
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                _ebayAPIAuthService.ParsedAuth.access_token
            );

            // Encoding of params is wrong, but API access works -- research docs
            string requestURL = QueryHelpers.AddQueryString(client.BaseAddress.ToString(), _baseRequestParams);

            string equestURL = QueryHelpers.AddQueryString(requestURL, new Dictionary<string, string>()
            {
                { "q", "drone" }
            });

            HttpRequestMessage requestToAPI = new HttpRequestMessage(
                HttpMethod.Get,
                requestURL
            );

            HttpResponseMessage responseFromAPI = await client.SendAsync(requestToAPI);

            if (responseFromAPI.IsSuccessStatusCode)
            {
                object parsedResponse = await responseFromAPI.Content.ReadFromJsonAsync<ParsedAuth>();
            }
            else
            {
                _logger.LogError(
                    $"Call to API failed. {(int)responseFromAPI.StatusCode}: {responseFromAPI.ReasonPhrase}"
                );
            }

            return new List<double>();
        }
    }
}