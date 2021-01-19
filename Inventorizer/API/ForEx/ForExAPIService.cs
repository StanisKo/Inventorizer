using System;
using System.Net.Http;
using System.Net.Http.Json;

using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

using Inventorizer.API.Base;

namespace Inventorizer.API.ForEx
{
    public class ForExAPIService : BaseAPI<ForExAPIService>, IHostedService
    {
        public ForExAPIService(IConfiguration configuration, IHttpClientFactory clientFactory, ILogger<ForExAPIService> logger)
            : base(configuration, clientFactory, logger)
        {

        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                _logger.LogError("Cancellation request is received before foreign exchange service started");

                cancellationToken.ThrowIfCancellationRequested();
            }

            RetrieveForeignExchangeRates();

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                _logger.LogError("Cancellation request is received before foreign exchange service stopped");

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
                object parsedExhangeRates = await responseFromForExAPI.Content.ReadFromJsonAsync<object>();

                Console.WriteLine(parsedExhangeRates);

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