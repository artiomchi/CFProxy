using System;
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
            services.AddHttpClient<CloudFlareClient>(client =>
            {
                client.BaseAddress = new Uri("https://api.cloudflare.com/client/v4/");
                client.DefaultRequestHeaders.Add("X-Proxy-User-Agent", "CFProxy (+https://cfproxy.com)");
            });
            _provider = services.BuildServiceProvider();
        }

        public static IServiceScope NewScope()
            => _provider.CreateScope();
    }
}
