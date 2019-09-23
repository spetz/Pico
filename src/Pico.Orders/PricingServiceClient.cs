using System;
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

        public async Task<PricingDto> GetAsync(Guid orderId)
        {
            var client = _clientFactory.CreateClient();
            var json = await client.GetStringAsync($"http://localhost:5002/orders/{orderId}/pricing");
            
            return string.IsNullOrWhiteSpace(json) ? null : JsonConvert.DeserializeObject<PricingDto>(json);
        }
    }
}