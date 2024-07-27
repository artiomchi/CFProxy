using System.Text.Json.Serialization;
using CFProxy.API.Dns;
using CFProxy.API.MyIP;

var builder = WebApplication.CreateSlimBuilder(args);


builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
});

builder.Services.AddHsts(options =>
{
    options.MaxAge = TimeSpan.FromDays(730);
});

builder.Services.AddCors(o => o
    .AddDefaultPolicy(b => b
        .WithOrigins(
            "https://" + builder.Configuration["mainHost"],
            "https://" + builder.Configuration["altHost"],
            "https://" + builder.Configuration["ipv4Host"],
            "https://" + builder.Configuration["ipv6Host"])));

builder.Services.AddHttpClient<IDnsClient, CloudFlareDnsClient>(client =>
{
    client.BaseAddress = new Uri("https://api.cloudflare.com/client/v4/");
    client.DefaultRequestHeaders.Add("X-Proxy-User-Agent", "CFProxy (+https://cfproxy.com)");
});

var app = builder.Build();

if (app.Environment.IsProduction())
{
    app.UseHsts();
}

app.UseCors();

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseDynDnsHandler();
app.UseMyIPHandler();

app.Run();

[JsonSerializable(typeof(IPAddressResponse))]
[JsonSerializable(typeof(MyIPConfig))]
internal partial class AppJsonSerializerContext : JsonSerializerContext { }
