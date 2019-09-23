using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace Pico.Orders
{
    public static class Extensions
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            IConfiguration configuration;
            using (var scope = services.BuildServiceProvider().CreateScope())
            {
                configuration = scope.ServiceProvider.GetService<IConfiguration>();
            }

            services.Configure<MessageBroker.Options>(configuration.GetSection("rabbitmq"));
            services.AddTransient<MessageBroker>();
            services.AddTransient<PricingServiceClient>();
            services.AddSingleton(sp =>
            {
                var options = sp.GetService<IOptions<MessageBroker.Options>>().Value;
                var connectionFactory = new ConnectionFactory
                {
                    HostName = options.HostName,
                    Port = options.Port,
                    VirtualHost = options.VirtualHost,
                    UserName = options.Username,
                    Password = options.Password,
                    Ssl = new SslOption()
                };

                return connectionFactory.CreateConnection("orders-service");
            });
            
            return services;
        }
    }
}