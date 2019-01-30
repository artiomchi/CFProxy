using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using CFProxy.API.Handlers;
using CFProxy.Repositories;
using Microsoft.AspNetCore.Http;

namespace CFProxy.API.CloudFlareProxy
{
    public class CloudFlareProxyMiddleware
    {
        private readonly RequestDelegate _next;

        public CloudFlareProxyMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, CloudFlareClient client, IProxyKeysRepository proxyKeysRepository)
        {
            using (var result = await client.SendRequest(
                new HttpMethod(context.Request.Method),
                context.Request.Path.Value.TrimStart('/'),
                context.TryGetRequestIPAddress(),
                context.GetRequestHeaders(),
                context.Request.ContentLength > 0 ? context.Request.Body : null))
            {
                context.Response.StatusCode = (int)result.StatusCode;
                foreach (var header in result.Headers)
                    context.Response.Headers[header.Key] = header.Value.ToArray();

                foreach (var header in result.Content.Headers)
                    context.Response.Headers[header.Key] = header.Value.ToArray();

                context.Response.Headers.Remove("transfer-encoding");
                using (var responseStream = await result.Content.ReadAsStreamAsync())
                {
                    await responseStream.CopyToAsync(context.Response.Body);
                }
            }
        }
    }
}
