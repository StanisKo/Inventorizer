using System.Net.Http;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace Inventorizer.API.Base
{
    /*
    Base API class that conceals some basic functionality
    and enforces child API classes to use it via DI
    */
    public abstract class BaseAPI<T>
    {
        protected readonly IConfiguration _configuration;

        protected readonly IHttpClientFactory _clientFactory;

        protected readonly ILogger<T> _logger;

        public BaseAPI(IConfiguration configuration, IHttpClientFactory clientFactory, ILogger<T> logger)
        {
            _configuration = configuration;
            _clientFactory = clientFactory;
            _logger = logger;
        }
    }
}