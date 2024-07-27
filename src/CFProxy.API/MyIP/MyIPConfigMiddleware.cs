namespace CFProxy.API.MyIP;

#pragma warning disable CS9113 // Parameter is unread.
public class MyIPConfigMiddleware(RequestDelegate _, IConfiguration configuration)
#pragma warning restore CS9113 // Parameter is unread.
{
    public async Task Invoke(HttpContext context)
    {
        var config = new MyIPConfig(configuration["ipv4Host"]!, configuration["ipv6Host"]!);

        await Results.Json(config, MyIPJsonGen.Default.MyIPConfig).ExecuteAsync(context);
    }
}
