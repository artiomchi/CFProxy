using CFProxy.API.DynDns;
using CFProxy.API.Extensions;

namespace Microsoft.AspNetCore.Builder;

public static class DynDnsMiddlewareExtensions
{
    public static IApplicationBuilder UseDynDnsHandler(this WebApplication app)
    {
        ArgumentNullException.ThrowIfNull(app);

        app.MapGet("/checkip", c => Results.Content(c.TryGetRequestIPAddress(), "text/plain").ExecuteAsync(c));

        app.Map("/nic/update", mapApp => mapApp.UseMiddleware<DynDnsMiddleware>());

        return app;
    }
}
