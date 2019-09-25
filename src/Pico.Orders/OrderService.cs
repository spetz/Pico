using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Pico.Orders
{
    public class OrderService
    {
        private readonly PricingServiceClient _pricingServiceClient;
        private readonly MessageBroker _messageBroker;
        private readonly ILogger<OrderService> _logger;

        public OrderService(PricingServiceClient pricingServiceClient, MessageBroker messageBroker,
            ILogger<OrderService> logger)
        {
            _pricingServiceClient = pricingServiceClient ?? throw new ArgumentNullException(nameof(pricingServiceClient));
            _messageBroker = messageBroker ?? throw new ArgumentNullException(nameof(messageBroker));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task CreateAsync(CreateOrder command)
        {
            if (string.IsNullOrWhiteSpace(command?.Client))
            {
                throw new ArgumentException("Invalid client.", nameof(command.Client));
            }
            
            var discount = await _pricingServiceClient.GetDiscountAsync(command.Client);
            _logger.LogInformation($"Received a discount ({discount.Discount}%) for order: {command.Id}");
            _messageBroker.Send(new {orderId = command.Id}, "orders", "order_created");
        }
    }
}