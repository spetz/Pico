using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Pico.Orders
{
    public class PricingServiceClient
    {
        private readonly IHttpClientFactory _clientFactory;

        public PricingServiceClient(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public async Task<DiscountDto> GetDiscountAsync(string client)
        {
            var httpClient = _clientFactory.CreateClient();
            var json = await httpClient.GetStringAsync($"http://localhost:5002/clients/{client}/discount");

            return string.IsNullOrWhiteSpace(json) ? null : JsonConvert.DeserializeObject<DiscountDto>(json);
        }
    }
}