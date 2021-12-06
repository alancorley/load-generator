using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using LoadGenerator.Interfaces;
using LoadGenerator.Services;
using System;
using System.IO;
using System.Threading.Tasks;


namespace LoadGenerator
{
    public sealed class Program
    {
        public static async Task Main(string[] args)
        {
            // add appsettings.json
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");

            var configuration = builder.Build();

            // add DIs
            var host = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    string uri = configuration.GetValue<string>("APIUri");
                    string apiKey = configuration.GetValue<string>("AuthKey");
                    services.AddScoped<ILoadGeneratorService, LoadGeneratorService>();
                    services.AddHttpClient();
                    services.AddHttpClient("generator", c =>
                    {
                        c.BaseAddress = new Uri(uri);
                        c.DefaultRequestHeaders.Add("x-api-key", apiKey);
                        c.Timeout = TimeSpan.FromSeconds(5);
                    });
                })
                .Build();

            // create instance of ILoadGeneratorService
            var svc = ActivatorUtilities.CreateInstance<LoadGeneratorService>(host.Services);
            await svc.Run();
            
        }


        
    }
}
