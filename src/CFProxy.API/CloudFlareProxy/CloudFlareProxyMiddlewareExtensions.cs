using System;
using Microsoft.AspNetCore.Builder;

namespace CFProxy.API.CloudFlareProxy
{
    public static class CloudFlareProxyMiddlewareExtensions
    {
        public static IApplicationBuilder UseCloudFlareProxyHandler(this IApplicationBuilder app)
        {
            if (app == null)
                throw new ArgumentNullException(nameof(app));

            app.Map("/client/v4", mapApp => mapApp.UseMiddleware<CloudFlareProxyMiddleware>());

            return app;
        }
    }
}
