using System.Net.Http;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

using Inventorizer.API.Base;

namespace Inventorizer.API.ForEx
{
    public class ForExAPIService : BaseAPI<ForExAPIService>
    {
        public ForExAPIService(IConfiguration configuration, IHttpClientFactory clientFactory, ILogger<ForExAPIService> logger)
            : base(configuration, clientFactory, logger)
        {

        }
    }
}