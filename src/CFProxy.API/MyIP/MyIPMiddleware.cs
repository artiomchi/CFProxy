using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CFProxy.API.Handlers;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace CFProxy.API.MyIP
{
    public class MyIPMiddleware
    {
        public MyIPMiddleware(RequestDelegate _) { }

        public async Task Invoke(HttpContext context)
        {
            var ipAddress = context.TryGetRequestIPAddress();
            if (ipAddress == null)
            {
                context.Response.StatusCode = 500;
                return;
            }

            if (context.Request.Query["v4"].Count > 0 && ipAddress.Contains(':'))
            {
                context.Response.StatusCode = 502;
                await context.Response.WriteAsync("Request should have been made over IPv4");
                return;
            }

            if (context.Request.Query["v6"].Count > 0 && !ipAddress.Contains(':'))
            {
                context.Response.StatusCode = 502;
                await context.Response.WriteAsync("Request should have been made over IPv6");
                return;
            }

            var format = context.Request.Query["format"];
            if (string.IsNullOrEmpty(format))
            {
                format = context.Request.Query["f"];
            }

            if (string.IsNullOrEmpty(format))
            {
                var supportedFormats = new[] { "json", "jsonp", "xml" };
                format = context.Request.Query.Keys
                    .Where(k => supportedFormats.Contains(k, StringComparer.OrdinalIgnoreCase))
                    .FirstOrDefault();
            }
            switch (format.ToString().ToUpperInvariant())
            {
                case "JSON":
                    {
                        context.Response.ContentType = "application/json";
                        var jsonIp = JsonConvert.SerializeObject(new { ip = ipAddress });
                        await context.Response.WriteAsync(jsonIp);
                        return;
                    }

                case "JSONP":
                    {
                        var methodName = context.Request.Query["callback"];
                        if (string.IsNullOrEmpty(methodName) || !Regex.IsMatch(methodName, @"^[a-z0-9]+$", RegexOptions.IgnoreCase))
                        {
                            context.Response.StatusCode = 400;
                            return;
                        }

                        context.Response.ContentType = "application/javascript";
                        var content = $"{methodName}('{ipAddress}')";
                        await context.Response.WriteAsync(content);
                        return;
                    }

                case "XML":
                    {
                        context.Response.ContentType = "application/xml";
                        var content = $"<myIP>{ipAddress}</myIP>";
                        await context.Response.WriteAsync(content);
                        return;
                    }

                default:
                    {
                        context.Response.ContentType = "text/plain";
                        await context.Response.WriteAsync(ipAddress);
                        return;
                    }
            }
        }
    }
}
