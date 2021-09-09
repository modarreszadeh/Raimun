using Microsoft.AspNetCore.Http;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Web.Infrastructure.Model;

namespace Web.Infrastructure
{
    public class HttpClientServices : IHttpClientServices
    {
        private readonly string _clientName;
        private readonly IHttpClientFactory _clientFactory;

        public HttpClientServices(IHttpClientFactory clientFactory, IOptions<WeatherApiSetting> options)
        {
            _clientFactory = clientFactory;
            _clientName = options.Value.ClientName;
        }

        public async Task<HttpResponseMessage> Get(string url)
        {
            var client = _clientFactory.CreateClient(_clientName);
            return await client.GetAsync(client.BaseAddress + url);
        }
    }

    public interface IHttpClientServices
    {
        Task<HttpResponseMessage> Get(string url);
    }
}