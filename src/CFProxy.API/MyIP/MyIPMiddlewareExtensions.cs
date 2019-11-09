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

            app.Map("/myip", mapApp => mapApp.UseMiddleware<MyIPMiddleware>());

            app.UseStaticFiles();

            return app;
        }
    }
}
