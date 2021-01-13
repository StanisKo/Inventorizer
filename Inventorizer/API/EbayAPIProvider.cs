using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Net.Http.Headers;

using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.WebUtilities;

using Inventorizer.API.Auth;

/*
TODO:

1. Figure out proper encoding
2. Request items concurrently (as Task -- look into TPL)
3. Better structure api dir
*/

namespace Inventorizer.API
{
    public class EbayAPIProvider : BaseAPI<EbayAPIProvider>
    {
        private readonly EbayAPIAuthService _ebayAPIAuthService;

        /*
        Unchanging request parameters that are applied to all items
        Since API expects strings for all params, we're using strings for numerical values
        */
        private readonly Dictionary<string, string> _baseRequestParams = new Dictionary<string, string>
        {
            /*
            To avoid unnecessary string operations, we do hardcode the filters in the format of:
            ?format=<param_1>:<value>,<param_N>:<value>
            */
            { "filter", "itemLocationCountry:DE,conditions:{USED}" },
            { "limit", "10" }
        };

        public string ErrorString { get; private set; }

        public EbayAPIProvider(IConfiguration configuration,
                               IHttpClientFactory clientFactory,
                               ILogger<EbayAPIProvider> logger,
                               EbayAPIAuthService ebayAPIAuthService) : base(configuration, clientFactory, logger)
        {
            _ebayAPIAuthService = ebayAPIAuthService;
        }

        public async Task<List<double>> RetrieveItemPrices(List<string> itemNames)
        {
            List<double> prices = new List<double>();

            HttpClient client = _clientFactory.CreateClient("EbayAPI");

            // Authenticate call with application access token
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                _ebayAPIAuthService.ParsedAuth.access_token
            );

            string requestURL = QueryHelpers.AddQueryString(
                client.BaseAddress.ToString(),
                new Dictionary<string, string>(_baseRequestParams)
            {
                /*
                Add baseParams with itemNames from the controller

                Q param from API is used for keyword search

                Different denominators apply different logic to provided keyword
                Comma results in AND logic

                For instance, ?q=running,shoes will retrieve items that mention running and shoes
                */
                { "q", String.Join(',', itemNames.First().Split(' ').Select(w => w.ToLower())) },
            });

            HttpRequestMessage requestToAPI = new HttpRequestMessage(
                HttpMethod.Get,
                requestURL
            );

            HttpResponseMessage responseFromAPI = await client.SendAsync(requestToAPI);

            if (responseFromAPI.IsSuccessStatusCode)
            {
                ParsedAPIResponse parsedAPIResponse = await responseFromAPI.Content
                    .ReadFromJsonAsync<ParsedAPIResponse>();

                prices = parsedAPIResponse.ItemSummaries
                    .Select(s => Convert.ToDouble(s.Price.Value))
                    .ToList();

                foreach (double price in prices)
                {
                    Console.WriteLine(price);
                }
            }
            else
            {
                _logger.LogError(
                    $"Call to API failed. {(int)responseFromAPI.StatusCode}: {responseFromAPI.ReasonPhrase}"
                );
            }

            return prices;
        }
    }
}