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

using Inventorizer.Shared;
using Inventorizer.API.Base;
using Inventorizer.API.Ebay.Auth;

namespace Inventorizer.API.Ebay.Provider
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
            ?filter=<param_1>:<value>,<param_N>:<value>
            */
            { "filter", "itemLocationCountry:DE,conditions:{USED}" },
            { "limit", "10" }
        };

        public EbayAPIProvider(IConfiguration configuration,
                               IHttpClientFactory clientFactory,
                               ILogger<EbayAPIProvider> logger,
                               EbayAPIAuthService ebayAPIAuthService) : base(configuration, clientFactory, logger)
        {
            _ebayAPIAuthService = ebayAPIAuthService;
        }

        /*
        Requests prices for provided item names
        Returns a collection of structs each containing item name
        and the prices of first 10 results for each name
        */
        public async Task <IEnumerable<MarketPrices>> RetrieveMarketPrices(IEnumerable<string> itemNames)
        {
            HttpClient client = _clientFactory.CreateClient("AllPurposeJsonAPI");

            // Authenticate calls with application access token
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                _ebayAPIAuthService.ParsedAuth.access_token
            );

            IEnumerable<Task<MarketPrices>> requestsToAPI = itemNames.Select(
                itemName => RetrievePricesForSingleItem(itemName, client)
            );

            IEnumerable<MarketPrices> marketPrices = await Task.WhenAll(requestsToAPI);

            return marketPrices;
        }

        /*
        Requests first 10 results for the provided item name,
        extracts their prices in a collection
        and returns a struct with an item name and the prices
        */
        private async Task <MarketPrices> RetrievePricesForSingleItem(string itemName, HttpClient client)
        {
            IEnumerable<double> marketPrices = new List<double>();

            string requestURL = QueryHelpers.AddQueryString(
                _configuration["EbayAPI:Base"],
                new Dictionary<string, string>(_baseRequestParams)
                {
                    /*
                    https://developer.ebay.com/api-docs/buy/browse/resources/item_summary/methods/search#_samples

                    q: <string> -- a string consisting of one or more keywords that are used to search for items;
                    if the keywords are separated by a comma, it is treated as an AND
                    */
                    { "q", String.Join(',', itemName.Split(' ').Select(w => w.ToLower())) },
                }
            );

            HttpRequestMessage requestToAPI = new HttpRequestMessage(
                HttpMethod.Get,
                requestURL
            );

            HttpResponseMessage responseFromAPI = await client.SendAsync(requestToAPI);

            if (responseFromAPI.IsSuccessStatusCode)
            {
                ParsedAPIResponse parsedAPIResponse = await responseFromAPI.Content
                    .ReadFromJsonAsync<ParsedAPIResponse>();

                // If no prices for the provided item name are available, return empty list
                marketPrices =
                    parsedAPIResponse.ItemSummaries?.Select(s => Convert.ToDouble(s.Price.Value)) ?? new List<double>();
            }
            else
            {
                string error =
                    $"Call to API failed. {(int)responseFromAPI.StatusCode}: {responseFromAPI.ReasonPhrase}";

                _logger.LogError(error);

                throw new Exception(error);
            }

            return new MarketPrices() { Name = itemName, Prices = marketPrices };
        }
    }
}