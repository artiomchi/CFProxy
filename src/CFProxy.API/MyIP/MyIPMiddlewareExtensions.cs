using CFProxy.API.MyIP;

namespace Microsoft.AspNetCore.Builder;

public static class MyIPMiddlewareExtensions
{
    public static WebApplication UseMyIPHandler(this WebApplication app)
    {
        ArgumentNullException.ThrowIfNull(app);

        app.Map("/myip/config", mapApp => mapApp.UseMiddleware<MyIPConfigMiddleware>());
        app.Map("/myip", mapApp => mapApp.UseMiddleware<MyIPMiddleware>());

        return app;
    }
}
