using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace Pico.Pricing
{
    public static class Program
    {
        public static Task Main(string[] args)
            => WebHost.CreateDefaultBuilder(args)
                .ConfigureServices(services => services
                    .AddMvcCore()
                    .AddJsonFormatters())
                .Configure(app => app.UseRouter(router => router
                    .MapGet("/", ctx => ctx.Response.WriteAsync("Pricing"))
                    .MapGet("orders/{orderId:guid}/pricing", ctx =>
                    {
                        var json = JsonConvert.SerializeObject(new
                        {
                            price = 100
                        });

                        return ctx.Response.WriteAsync(json);
                    })))
                .Build()
                .RunAsync();
    }
}