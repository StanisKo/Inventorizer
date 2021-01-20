using System;
using System.Net.Http;
using System.Net.Http.Json;

using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

using Inventorizer.API.Base;

/*
Since stats needs an access to initial item prices (and not only market prices),
find a way to provide prices controller with initial prcies without involving FE

Send a signal in item controller, call pricesController from item controller?

And if so -- how does FE will know that is safe to fetch?

Also, rename itemPrices to marketPrices where needed

Worst case scenario -- add prices to the request payload on the FE

EbayAPIProvider should have a field to store { name, price } of items
Write to that field from item controller

And when EbayAPIProvider will be called from PricesController, it will use internal params
instead of those passed to it from AJAX request
*/

namespace Inventorizer.API.ForEx
{
    public class ForExAPIService : BaseAPI<ForExAPIService>, IHostedService
    {
        public ParsedExchangeRate ParsedExchangeRate { get; private set; }

        public ForExAPIService(IConfiguration configuration, IHttpClientFactory clientFactory, ILogger<ForExAPIService> logger)
            : base(configuration, clientFactory, logger)
        {

        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                _logger.LogError("Cancellation request was received before foreign exchange service started");

                cancellationToken.ThrowIfCancellationRequested();
            }

            RetrieveForeignExchangeRates();

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                _logger.LogError("Cancellation request was received before foreign exchange service stopped");

                cancellationToken.ThrowIfCancellationRequested();
            }

            return Task.CompletedTask;
        }

        private async void RetrieveForeignExchangeRates()
        {
            HttpClient client = _clientFactory.CreateClient("AllPurposeJsonAPI");

            HttpRequestMessage requestToForExAPI = new HttpRequestMessage(
                HttpMethod.Get,
                _configuration["ForExAPI:Base"]
            );

            HttpResponseMessage responseFromForExAPI = await client.SendAsync(requestToForExAPI);

            if (responseFromForExAPI.IsSuccessStatusCode)
            {
                ParsedExchangeRate = await responseFromForExAPI.Content.ReadFromJsonAsync<ParsedExchangeRate>();

                _logger.LogInformation("Foreign exchange rates retrieved successfully");
            }
            else
            {
                _logger.LogError(
                    $"Retrieving exhange rates failed. {(int)responseFromForExAPI.StatusCode}: {responseFromForExAPI.ReasonPhrase}"
                );
            }
        }
    }
}