using System.Net.Http;
using System.Net.Http.Json;
using System.Collections.Generic;

using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.WebUtilities;

using Inventorizer.API.Base;

namespace Inventorizer.API.ForEx
{
    /*
    A background service that retrieves latest exchange rates on boot
    and makes them avaialable for the Stats module via DI
    */
    public class ForExAPIService : BaseAPI<ForExAPIService>, IHostedService
    {
        // Retrieve currencies already quoted against USD to avoid unnecessary computations
        private const string _quoteAgainst = "USD";

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

            string requestURL = QueryHelpers.AddQueryString(
                _configuration["ForExAPI:Base"],
                new Dictionary<string, string>() { { "base", _quoteAgainst } }
            );

            HttpRequestMessage requestToForExAPI = new HttpRequestMessage(
                HttpMethod.Get,
                requestURL
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