using System;
using System.Text;
using System.Net.Http;
using System.Net.Http.Json;
using System.Net.Http.Headers;

using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

using Inventorizer.API.Base;

namespace Inventorizer.API.Ebay.Auth
{
    /*
    A background service that retrieves application access token on boot
    and remints it based on interval provided by ebay authentication service

    Makes token available to EbayAPIProvider via DI
    */
    public class EbayAPIAuthService : BaseAPI<EbayAPIAuthService>, IHostedService
    {
        private string _clientId;
        private string _clientSecret;

        private int _numberOfAuthRequests = 0;
        private const int _MAX_AUTH_REQUESTS = 1;

        private Timer _authRequestTimer;

        public ParsedAuth ParsedAuth { get; private set; }

        public EbayAPIAuthService(IConfiguration configuration, IHttpClientFactory clientFactory, ILogger<EbayAPIAuthService> logger)
            : base(configuration, clientFactory, logger)
        {
            _clientId = _configuration["ClientId"];
            _clientSecret = _configuration["ClientSecret"];
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                _logger.LogError("Cancellation request was received before authentication service started");

                cancellationToken.ThrowIfCancellationRequested();
            }

            /*
            Token expiration interval is provided by authentication service (7200 seconds)

            Since there is no token in scope on startup, we launch job with 0 seconds interval
            and lock request until it's finished (see callback)

            After the token is retrieved we change the interval to value provided by API
            */
            _authRequestTimer = new Timer(RetrieveApplicationAccessToken, null, 0, 0);

            int intervalFromAPI = (int)TimeSpan.FromSeconds(ParsedAuth.expires_in).TotalMilliseconds;

            _authRequestTimer.Change(0, intervalFromAPI);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                _logger.LogError("Cancellation request was received before authentication service stopped");

                cancellationToken.ThrowIfCancellationRequested();
            }

            // Change the start time and interval to infinite, therefore killing the timer
            _authRequestTimer?.Change(Timeout.Infinite, Timeout.Infinite);

            return Task.CompletedTask;
        }

        private async void RetrieveApplicationAccessToken(object state)
        {
            if (_numberOfAuthRequests < _MAX_AUTH_REQUESTS)
            {
                Interlocked.Increment(ref _numberOfAuthRequests);

                HttpClient client = _clientFactory.CreateClient("AllPurposeJsonAPI");

                /*
                Encode portal-provided application keys as base64 string and add to headers
                in order to access application token from OAuth
                */
                byte[] byteArray = Encoding.ASCII.GetBytes($"{_clientId}:{_clientSecret}");

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                    "Basic",
                    Convert.ToBase64String(byteArray)
                );

                // Specify grant type and scope of available API
                var requestParams = new List<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>("grant_type", _configuration["EbayAPI:GrantType"]),
                    new KeyValuePair<string, string>("scope", _configuration["EbayAPI:Scope"])
                };

                HttpRequestMessage requestToAuth = new HttpRequestMessage(
                    HttpMethod.Post,
                    _configuration["EbayAPI:Auth"]
                );

                // Encode requst params into url and specify required by the API Content-Type header
                requestToAuth.Content = new FormUrlEncodedContent(requestParams);

                requestToAuth.Content.Headers.ContentType = new MediaTypeHeaderValue(
                    "application/x-www-form-urlencoded"
                );

                HttpResponseMessage responseFromAuth = await client.SendAsync(requestToAuth);

                if (responseFromAuth.IsSuccessStatusCode)
                {
                    ParsedAuth = await responseFromAuth.Content.ReadFromJsonAsync<ParsedAuth>();

                    _logger.LogInformation("Application access token retrieved successfully");
                }
                else
                {
                    _authRequestTimer.Change(Timeout.Infinite, Timeout.Infinite);

                    _logger.LogError(
                        $"Auth failed. {(int)responseFromAuth.StatusCode}: {responseFromAuth.ReasonPhrase}"
                    );
                }

                Interlocked.Decrement(ref _numberOfAuthRequests);
            }
        }
    }
}