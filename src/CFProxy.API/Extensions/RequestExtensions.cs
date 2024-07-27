namespace CFProxy.API.Extensions;

public static class RequestExtensions
{
    public static string? TryGetHeader(this HttpContext context, string header)
        => context.Request.Headers.TryGetValue(header, out Microsoft.Extensions.Primitives.StringValues value)
        ? (string?)value
        : null;

    private const string ForwardedForHeader = "X-Forwarded-For";

    public static string? TryGetRequestIPAddress(this HttpContext context)
        => context.Request.Headers.TryGetValue(ForwardedForHeader, out Microsoft.Extensions.Primitives.StringValues value)
        ? value.ToArray().FirstOrDefault()?.Split(',').FirstOrDefault()
        : context.Connection.RemoteIpAddress?.ToString();
}
