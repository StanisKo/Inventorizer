using System;
using System.Text;
using System.Net.Http;
using System.Net.Http.Json;
using System.Net.Http.Headers;

using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;

namespace Inventorizer.API
{
    public class EbayAPI : IHostedService
    {
        private readonly IConfiguration _configuration;

        private readonly IHttpClientFactory _clientFactory;

        private string _clientId;
        private string _clientSecret;

        private const int _MAX_AUTH_REQUESTS = 1;
        private int _numberOfAuthRequests = 0;

        private Timer _authRequestTimer;

        private ParsedAuth _parsedAuth;

        public string ErrorString { get; private set; }

        public EbayAPI(IConfiguration configuration, IHttpClientFactory clientFactory)
        {
            _configuration = configuration;
            _clientFactory = clientFactory;

            _clientId = _configuration["ClientId"];
            _clientSecret = _configuration["ClientSecret"];
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            // Cancellation token workflow here

            /*
            Token expiration interval is provided by authentication service (7200 seconds)

            Since there is no token in scope on startup, we launch job with 0 seconds interval
            and lock request in one thread until it's finished (see callback)

            After the token is retrieved the second timer will launch but already with the interval
            value provided by API
            */
            if (ErrorString == null)
            {
                Timer _authRequestTimer = new Timer(RetrieveApplicationAccessToken, null, 0, 0);

                // Change interval only if token was retrieved (so only once -- on 200 OK from first request)
                if (!String.IsNullOrEmpty(_parsedAuth.access_token))
                {
                    int intervalFromAPI = (int)TimeSpan.FromSeconds(_parsedAuth.expires_in).TotalMilliseconds;

                    _authRequestTimer?.Change(0, intervalFromAPI);
                }
            }
            else
            {
                _authRequestTimer?.Change(Timeout.Infinite, Timeout.Infinite);
            }

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            // Change the start time to infinite, therefore killing the timer
            _authRequestTimer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public async Task<List<double>> RetrieveItemPrices(List<string> itemNames)
        {
            return new List<double>();
        }

        private async void RetrieveApplicationAccessToken(object state)
        {
            if (_numberOfAuthRequests < _MAX_AUTH_REQUESTS)
            {
                Interlocked.Increment(ref _numberOfAuthRequests);

                HttpClient client = _clientFactory.CreateClient("EbayAPI");

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
                    _parsedAuth = await responseFromAuth.Content.ReadFromJsonAsync<ParsedAuth>();

                    ErrorString = null;
                }
                else
                {
                    ErrorString =
                        $"Auth failed. {(int)responseFromAuth.StatusCode}: {responseFromAuth.ReasonPhrase}";
                }

                Interlocked.Decrement(ref _numberOfAuthRequests);
            }
        }
    }
}