using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace Pico.Orders
{
    public static class Program
    {
        public static Task Main(string[] args)
            => WebHost.CreateDefaultBuilder(args)
                .ConfigureServices(services => services
                    .AddServices()
                    .AddHttpClient()
                    .AddMvcCore()
                    .AddJsonFormatters())
                .Configure(app => app.UseRouter(router => router
                    .MapGet("/", ctx => ctx.Response.WriteAsync("Orders"))
                    .MapPost("orders", async ctx =>
                    {
                        if (ctx.Request.Body is null)
                        {
                            ctx.Response.StatusCode = 400;
                            return;
                        }

                        string json;
                        using (var streamReader = new StreamReader(ctx.Request.Body))
                        {
                            json = await streamReader.ReadToEndAsync();
                        }

                        if (string.IsNullOrWhiteSpace(json))
                        {
                            ctx.Response.StatusCode = 400;
                            return;
                        }
                        
                        var order = JsonConvert.DeserializeObject<Order>(json);
                        if (string.IsNullOrWhiteSpace(order.Name))
                        {
                            ctx.Response.StatusCode = 400;
                            return;
                        }

                        order.Id = Guid.NewGuid();
                        var pricingServiceClient = app.ApplicationServices.GetRequiredService<PricingServiceClient>();
                        var pricing = await pricingServiceClient.GetAsync(order.Id);
                        var messageBroker = app.ApplicationServices.GetRequiredService<MessageBroker>();
                        messageBroker.Send(new {orderId = order.Id}, "orders", "order_created");
                        ctx.Response.StatusCode = 201;
                        ctx.Response.Headers.Add("Location", $"orders/{order.Id}");
                    })))
                .Build()
                .RunAsync();
    }
}