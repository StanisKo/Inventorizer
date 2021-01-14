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

/*
TODO:

1. Request items concurrently (as Task -- look into TPL)

NOTE:

Stat service will have to translate USD to EUR since all prices are in USD
*/

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

        // https://www.michalbialecki.com/2018/04/19/how-to-send-many-requests-in-parallel-in-asp-net-core/!
        public async Task<List<ItemNameAndItsPrices>> RetrieveItemPrices(List<string> itemNames)
        {
            HttpClient client =_clientFactory.CreateClient("EbayAPI");

            IEnumerable<Task> requestsToAPI = itemNames.Select(
                itemName => RetrievePricesForSingeItem(itemName, client)
            );

            List<ItemNameAndItsPrices> itemPrices = await Task.WhenAll(requestsToAPI);

            return itemPrices;
        }

        private async Task<ItemNameAndItsPrices> RetrievePricesForSingeItem(string itemName, HttpClient client)
        {
            List<double> prices = new List<double>();

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
                { "q", String.Join(',', itemName.Split(' ').Select(w => w.ToLower())) },
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

            return new ItemNameAndItsPrices()
            {
                ItemName = itemName,
                ItemPrices = prices
            };
        }
    }
}