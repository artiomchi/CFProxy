using CFProxy.API.Handlers;
using Microsoft.Extensions.DependencyInjection;

namespace CFProxy.API.AzureFunc
{
    static class Startup
    {
        private static readonly ServiceProvider _provider;

        static Startup()
        {
            var services = new ServiceCollection();
            services.AddHttpClient<CloudFlareClient>(CloudFlareClient.ConfigureClient);
            _provider = services.BuildServiceProvider();
        }

        public static IServiceScope NewScope()
            => _provider.CreateScope();
    }
}
