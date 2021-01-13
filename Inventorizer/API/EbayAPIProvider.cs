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

/*
TODO:

1. Figure out proper encoding
2. Fifure out response shape
3. Request items concurrently (as Task -- look into TPL)
*/

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

        public async Task<List<double>> RetrieveItemPrices(List<string> itemNames)
        {
            HttpClient client = _clientFactory.CreateClient("EbayAPI");

            // Authenticate call with application access token
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                _ebayAPIAuthService.ParsedAuth.access_token
            );

            string requestURL = QueryHelpers.AddQueryString(
                client.BaseAddress.ToString(),
                new Dictionary<string, string>()
            {
                { "q", "drone" },
                {"limit", "10"}
            });

            HttpRequestMessage requestToAPI = new HttpRequestMessage(
                HttpMethod.Get,
                requestURL
            );

            HttpResponseMessage responseFromAPI = await client.SendAsync(requestToAPI);

            if (responseFromAPI.IsSuccessStatusCode)
            {
                ParsedAPIResponse parsedAPIResponse = await responseFromAPI.Content.ReadFromJsonAsync<ParsedAPIResponse>();

                foreach (ItemSummary itemSummary in parsedAPIResponse.ItemSummaries)
                {
                    Console.WriteLine(itemSummary.Price.Value);
                }
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