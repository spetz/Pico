using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Pico.Deliveries
{
    public static class Program
    {
        public static Task Main(string[] args)
            => WebHost.CreateDefaultBuilder(args)
                .ConfigureServices(services => services
                    .AddServices()
                    .AddHostedService<MessageProcessor>()
                    .AddMvcCore()
                    .AddJsonFormatters())
                .Configure(app => app.UseRouter(router => router
                    .MapGet("/", ctx => ctx.Response.WriteAsync("Deliveries"))))
                .Build()
                .RunAsync();
    }
}