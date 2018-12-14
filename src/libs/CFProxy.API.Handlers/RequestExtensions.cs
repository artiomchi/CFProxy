using System.Linq;
using System.Net.Http;
using Microsoft.AspNetCore.Http;

namespace CFProxy.API.Handlers
{
    public static class RequestExtensions
    {
        public static string TryGetHeader(this HttpContext context, string header)
            => context.Request.Headers.ContainsKey(header)
            ? (string)context.Request.Headers[header]
            : null;

        public static string TryGetHeader(this HttpRequestMessage request, string header)
            => request.Headers.TryGetValues(header, out var headerValues)
            ? string.Join(" ", headerValues)
            : null;

        private const string ForwardedForHeader = "X-Forwarded-For";

        public static string TryGetRequestIPAddress(this HttpContext context)
            => context.Request.Headers.ContainsKey(ForwardedForHeader)
            ? context.Request.Headers[ForwardedForHeader].ToArray().FirstOrDefault()?.Split(',').FirstOrDefault()
            : context.Connection.RemoteIpAddress.ToString();

        public static string TryGetRequestIPAddress(this HttpRequestMessage request)
            => request.Headers.TryGetValues(ForwardedForHeader, out var headerValues)
            ? headerValues.FirstOrDefault()?.Split(',').FirstOrDefault()
            : request.Properties.Values.OfType<HttpContext>().FirstOrDefault()?.TryGetRequestIPAddress();
    }
}
