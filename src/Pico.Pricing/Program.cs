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
                    .AddTransient<DiscountService>()
                    .AddMvcCore()
                    .AddJsonFormatters())
                .Configure(app => app.UseRouter(router => router
                    .MapGet("/", ctx => ctx.Response.WriteAsync("Pricing"))
                    .MapGet("clients/{client}/discount", ctx =>
                    {
                        var client = ctx.GetRouteValue("client").ToString();
                        var discount = ctx.RequestServices.GetRequiredService<DiscountService>().GetDiscount(client);

                        return ctx.Response.WriteAsync(JsonConvert.SerializeObject(new
                        {
                            discount
                        }));
                    })))
                .Build()
                .RunAsync();
    }
}