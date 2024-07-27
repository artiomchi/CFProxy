using CFProxy.API.Extensions;
using System.Text.RegularExpressions;

namespace CFProxy.API.MyIP;

#pragma warning disable CS9113 // Parameter is unread.
public partial class MyIPMiddleware(RequestDelegate _)
#pragma warning restore CS9113 // Parameter is unread.
{
    [GeneratedRegex(@"^[a-z0-9]+$", RegexOptions.IgnoreCase)]
    private static partial Regex LettersOrNumbersRegex();

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
                    var response = new IPAddressResponse(ipAddress);
                    await Results.Json(response, MyIPJsonGen.Default.IPAddressResponse).ExecuteAsync(context);
                    return;
                }

            case "JSONP":
                {
                    string? methodName = context.Request.Query["callback"];
                    if (string.IsNullOrEmpty(methodName) || !LettersOrNumbersRegex().IsMatch(methodName))
                    {
                        context.Response.StatusCode = 400;
                        return;
                    }

                    var content = $"{methodName}('{ipAddress}')";
                    await Results.Content(content, "application/javascript").ExecuteAsync(context);
                    return;
                }

            case "XML":
                {
                    var content = $"<myIP>{ipAddress}</myIP>";
                    await Results.Content(content, "application/xml").ExecuteAsync(context);
                    return;
                }

            default:
                {
                    await Results.Content(ipAddress, "text/plain").ExecuteAsync(context);
                    return;
                }
        }
    }
}
