using CFProxy.API.CloudFlareProxy;

namespace Microsoft.AspNetCore.Builder;

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
