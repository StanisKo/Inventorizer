using System.Net.Http;

namespace Inventorizer.API
{
    public interface IBaseAPI
    {
        IHttpClientFactory clientFactory { get; }
    }
}