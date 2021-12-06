using LoadGenerator.Interfaces;
using LoadGenerator.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Tests
{
    public class TestFixture : IAsyncDisposable
    {
        public ServiceProvider ServiceProvider { get; set; }

        public TestFixture()
        {
            IServiceCollection services = new ServiceCollection();

            IConfiguration config = SetupConfig();

            SetupIoC(services, config);

            ServiceProvider = services.BuildServiceProvider();
        }

        public static IConfiguration SetupConfig()
        {
            IConfigurationBuilder builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");
            
            return builder.Build();
        }

        private static void SetupIoC(IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IConfiguration>(configuration);
            
            services.AddScoped<ILoadGeneratorService, LoadGeneratorService>();


            string uri = configuration.GetValue<string>("APIUri");
            string apiKey = configuration.GetValue<string>("AuthKey");
            services.AddHttpClient("generator", c =>
            {
                c.BaseAddress = new Uri(uri);
                c.DefaultRequestHeaders.Add("x-api-key", apiKey);
            });

        }

        public ValueTask DisposeAsync()
        {
            return default;
        }

    }
}