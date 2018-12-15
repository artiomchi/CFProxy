using System.Threading.Tasks;
using CFProxy.API.Handlers;
using CFProxy.API.Handlers.DynDns;
using Microsoft.AspNetCore.Http;

namespace CFProxy.API.DynDns
{
    public class DynDnsMiddleware
    {
        private readonly RequestDelegate _next;

        public DynDnsMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, NicUpdateHandler handler)
        {
            var response = await handler.Process(context);
            await context.Response.WriteAsync(response);
        }
    }
}
