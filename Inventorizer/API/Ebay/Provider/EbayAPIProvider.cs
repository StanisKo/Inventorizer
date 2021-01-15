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
        Requests prices for all items present in the system
        Returns a collection of structs each containing item name and the prices of first 10 matches
        */
        public async Task<IEnumerable<ItemNameAndItsPrices>> RetrieveItemPrices(IEnumerable<string> itemNames)
        {
            HttpClient client =_clientFactory.CreateClient("AllPurposeJsonAPI");

            IEnumerable<Task<ItemNameAndItsPrices>> requestsToAPI = itemNames.Select(
                itemName => RetrievePricesForSingleItem(itemName, client)
            );

            IEnumerable<ItemNameAndItsPrices> itemPrices = await Task.WhenAll(requestsToAPI);

            return itemPrices;
        }

        /*
        Requests first 10 matches for the provided item name,
        extract their prices in a collection and returns a struct with an item name and the prices
        */
        private async Task<ItemNameAndItsPrices> RetrievePricesForSingleItem(string itemName, HttpClient client)
        {
            IEnumerable<double> itemPrices = new List<double>();

            // Authenticate call with application access token
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                _ebayAPIAuthService.ParsedAuth.access_token
            );

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

                itemPrices = parsedAPIResponse.ItemSummaries.Select(s => Convert.ToDouble(s.Price.Value));
            }
            else
            {
                string error =
                    $"Call to API failed. {(int)responseFromAPI.StatusCode}: {responseFromAPI.ReasonPhrase}";

                _logger.LogError(error);

                throw new Exception(error);
            }

            return new ItemNameAndItsPrices()
            {
                ItemName = itemName,
                ItemPrices = itemPrices
            };
        }
    }
}