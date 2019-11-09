using System;
using CFProxy.API.DynDns;
using CFProxy.API.Handlers;
using Microsoft.AspNetCore.Http;

namespace Microsoft.AspNetCore.Builder
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
                        context.TryGetRequestIPAddress())));

            app.Map("/nic/update", mapApp => mapApp.UseMiddleware<DynDnsMiddleware>());

            return app;
        }
    }
}
