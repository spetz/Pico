using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace Pico.Deliveries
{
    public static class Extensions
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            
            IConfiguration configuration;
            using (var scope = services.BuildServiceProvider().CreateScope())
            {
                configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
            }

            services.Configure<MessageBroker.Options>(configuration.GetSection("rabbitmq"));
            services.AddTransient<MessageBroker>();
            services.AddSingleton(sp =>
            {
                var options = sp.GetRequiredService<IOptions<MessageBroker.Options>>().Value;
                var connectionFactory = new ConnectionFactory
                {
                    HostName = options.HostName,
                    Port = options.Port,
                    VirtualHost = options.VirtualHost,
                    UserName = options.Username,
                    Password = options.Password,
                    Ssl = new SslOption()
                };

                return connectionFactory.CreateConnection("deliveries-service");
            });
            
            return services;
        }
    }
}