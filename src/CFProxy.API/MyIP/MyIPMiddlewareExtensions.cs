using System;
using CFProxy.API.MyIP;

namespace Microsoft.AspNetCore.Builder
{
    public static class MyIPMiddlewareExtensions
    {
        public static IApplicationBuilder UseMyIPHandler(this IApplicationBuilder app)
        {
            if (app == null)
                throw new ArgumentNullException(nameof(app));

            app.Map("/myip/config", mapApp => mapApp.UseMiddleware<MyIPConfigMiddleware>());
            app.Map("/myip", mapApp => mapApp.UseMiddleware<MyIPMiddleware>());

            app.UseDefaultFiles();
            app.UseStaticFiles();

            return app;
        }
    }
}
