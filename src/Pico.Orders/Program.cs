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
                        var command = await ctx.ReadBodyAsync<CreateOrder>();
                        if (command is null)
                        {
                            ctx.Response.StatusCode = 400;
                            return;
                        }

                        var orderService = app.ApplicationServices.GetRequiredService<OrderService>();
                        await orderService.CreateAsync(command);
                        ctx.Response.StatusCode = 201;
                        ctx.Response.Headers.Add("Location", $"orders/{command.Id}");
                    })))
                .Build()
                .RunAsync();
    }
}