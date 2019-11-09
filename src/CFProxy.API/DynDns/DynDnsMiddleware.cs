using System.Threading.Tasks;
using CFProxy.API.Handlers;
using CFProxy.API.Handlers.DynDns;
using Microsoft.AspNetCore.Http;

namespace CFProxy.API.DynDns
{
    public class DynDnsMiddleware
    {
        public DynDnsMiddleware(RequestDelegate _) { }

        public async Task Invoke(HttpContext context, NicUpdateHandler handler)
        {
            var response = await handler.Process(context);
            await context.Response.WriteAsync(response);
        }
    }
}
