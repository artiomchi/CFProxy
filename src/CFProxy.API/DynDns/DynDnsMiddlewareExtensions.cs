using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace CFProxy.API.DynDns
{
    public static class DynDnsMiddlewareExtensions
    {
        public static IApplicationBuilder UseDynDnsHandler(this IApplicationBuilder app)
        {
            if (app == null)
                throw new ArgumentNullException(nameof(app));

            app.Map("/checkip", mapApp
                => mapApp.Run(context
                    => context.Response.WriteAsync(
                        context.Connection.RemoteIpAddress.ToString())));

            app.Map("/nic/update", mapApp => mapApp.UseMiddleware<DynDnsMiddleware>());

            return app;
        }
    }
}
