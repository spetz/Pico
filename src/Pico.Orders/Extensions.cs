using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
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
            services.AddTransient<OrderService>();
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

        public static async Task<T> ReadBodyAsync<T>(this HttpContext ctx) where T : class
        {
            if (ctx.Request.Body is null)
            {
                return default;
            }

            string json;
            using (var streamReader = new StreamReader(ctx.Request.Body))
            {
                json = await streamReader.ReadToEndAsync();
            }

            return string.IsNullOrWhiteSpace(json) ? default : JsonConvert.DeserializeObject<T>(json);
        }
    }
}