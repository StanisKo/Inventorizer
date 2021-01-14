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

2. Request in batches to speed things up

3. Implement pagination

4. !use IQuerable/IEnumerable = -- read from post tutorials! Since iterating over it is much faster than iterating over list
(
    this is also connected to pagination,
    since IQuerable puts the work on the database,
    while IEnumerable loads everything into memory

    Therefore, if you need to sort or filter the collection, or limit it, use IQueyrable
)

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

        public EbayAPIProvider(IConfiguration configuration,
                               IHttpClientFactory clientFactory,
                               ILogger<EbayAPIProvider> logger,
                               EbayAPIAuthService ebayAPIAuthService) : base(configuration, clientFactory, logger)
        {
            _ebayAPIAuthService = ebayAPIAuthService;
        }

        public async Task<IEnumerable<ItemNameAndItsPrices>> RetrieveItemPrices(IEnumerable<string> itemNames)
        {
            HttpClient client =_clientFactory.CreateClient("EbayAPI");

            IEnumerable<Task<ItemNameAndItsPrices>> requestsToAPI = itemNames.Select(
                itemName => RetrievePricesForSingeItem(itemName, client)
            );

            IEnumerable<ItemNameAndItsPrices> itemPrices = await Task.WhenAll(requestsToAPI);

            return itemPrices;
        }

        private async Task<ItemNameAndItsPrices> RetrievePricesForSingeItem(string itemName, HttpClient client)
        {
            IEnumerable<double> itemPrices = new List<double>();

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

                itemPrices = parsedAPIResponse.ItemSummaries.Select(s => Convert.ToDouble(s.Price.Value));
            }
            else
            {
                _logger.LogError(
                    $"Call to API failed. {(int)responseFromAPI.StatusCode}: {responseFromAPI.ReasonPhrase}"
                );

                // throw here and handle in controller
            }

            return new ItemNameAndItsPrices()
            {
                ItemName = itemName,
                ItemPrices = itemPrices
            };
        }
    }
}