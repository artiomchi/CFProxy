using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CFProxy.API.Handlers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace CFProxy.API.MyIP
{
    public class MyIPMiddleware
    {
        private readonly IConfiguration _configuration;

        public MyIPMiddleware(RequestDelegate _, IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task Invoke(HttpContext context)
        {
            var ipAddress = context.TryGetRequestIPAddress();
            if (ipAddress == null)
            {
                context.Response.StatusCode = 500;
                return;
            }

            var format = context.Request.Query["format"];
            if (string.IsNullOrEmpty(format) && context.Request.QueryString.HasValue)
                format = context.Request.QueryString.Value.TrimStart('?');
            switch (format.ToString().ToUpperInvariant())
            {
                case "JSON":
                    {
                        context.Response.ContentType = "application/json";
                        context.Response.Headers.Add("Access-Control-Allow-Origin", "https://" + _configuration["mainHost"]);
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
