using System;
using Microsoft.Extensions.Logging;

namespace Pico.Pricing
{
    public class DiscountService
    {
        private readonly ILogger<DiscountService> _logger;

        public DiscountService(ILogger<DiscountService> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        public decimal GetDiscount(string client)
        {
            _logger.LogInformation($"Calculating a discount for client: {client}");
            if (string.IsNullOrWhiteSpace(client))
            {
                return 0;
            }
            
            switch (client)
            {
                case "partner": return 20;
                case "vip": return 10;
                default: return 5;
            }
        }
    }
}