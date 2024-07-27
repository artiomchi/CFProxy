using CFProxy.API.Dns;
using CFProxy.API.Extensions;

namespace CFProxy.API.CloudFlareProxy;

#pragma warning disable CS9113 // Parameter is unread.
public class CloudFlareProxyMiddleware(RequestDelegate _)
#pragma warning restore CS9113 // Parameter is unread.
{
    public async Task Invoke(HttpContext context, IDnsClient client)
    {
        using var request = new HttpRequestMessage
        {
            Method = new HttpMethod(context.Request.Method),
            RequestUri = new Uri(context.Request.Path.Value?.TrimStart('/') ?? "", UriKind.Relative),
        };
        var requestIP = context.TryGetRequestIPAddress();
        if (requestIP != null)
            request.Headers.TryAddWithoutValidation("X-Forwarded-For", requestIP);

        if (context.Request.ContentLength > 0)
        {
            request.Content = new StreamContent(context.Request.Body);
            foreach (var header in context.Request.Headers)
                request.Content.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray());
        }

        using var result = await client.Client.SendAsync(request);
        context.Response.StatusCode = (int)result.StatusCode;
        foreach (var header in result.Headers)
            context.Response.Headers[header.Key] = header.Value.ToArray();

        foreach (var header in result.Content.Headers)
            context.Response.Headers[header.Key] = header.Value.ToArray();

        context.Response.Headers.Remove("transfer-encoding");
        using var responseStream = await result.Content.ReadAsStreamAsync();
        await responseStream.CopyToAsync(context.Response.Body);
    }
}
