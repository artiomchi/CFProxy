using System.Net.Http.Headers;
using System.Text;
using CFProxy.API.Dns;
using CFProxy.API.Extensions;

namespace CFProxy.API.DynDns;

#pragma warning disable CS9113 // Parameter is unread.
public class DynDnsMiddleware(RequestDelegate _)
#pragma warning restore CS9113 // Parameter is unread.
{
    public async Task Invoke(HttpContext context, IDnsClient dnsClient)
    {
        var authorizationHeader = context.TryGetHeader("Authorization");
        var userAgentHeader = context.TryGetHeader("User-Agent");
        var requestIP = context.TryGetRequestIPAddress();
        var myip = (string?)context.Request.Query["myip"] ?? requestIP;
        var hostname = context.Request.Query["hostname"];

        var response = await Process(dnsClient, authorizationHeader, userAgentHeader, requestIP, myip, hostname);
        await context.Response.WriteAsync(response);
    }

    public async Task<string> Process(IDnsClient client, string? authHeader, string? userAgent, string? requestIP, string? myip, string? hostname)
    {
        var (valid, login, password) = GetBasicAuth(authHeader);
        if (!valid || login is null || password is null)
            return "badauth";

        if (string.IsNullOrWhiteSpace(hostname))
            return "nohost";

        if (string.IsNullOrWhiteSpace(myip))
            return "911";

        client.Authenticate(login, password, userAgent, requestIP);

        var zones = await client.GetZones();
        var zone = zones
            .Where(z => hostname.EndsWith(z.Name, StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(z => z.Name.Length)
            .FirstOrDefault();
        if (zone?.Name == null)
            return "911";

        var recordType = myip.IndexOf(':') > 0 ? "AAAA" : "A";

        var records = (await client.GetRecords(zone.Id, recordType, hostname))
            ?.Where(r => string.Equals(r.Name, hostname, StringComparison.OrdinalIgnoreCase))
            .ToArray();
        if (records == null)
            return "911";
        if (records.Length == 0)
        {
            var createSuccess = await client.CreateRecord(zone.Id, recordType, hostname, myip);

            return createSuccess
                ? "ok " + myip
                : "911";
        }

        foreach (var record in records.Skip(1))
            await client.DeleteRecord(zone.Id, record.Id);

        var updateSuccess = await client.UpdateRecord(zone.Id, records[0].Id, recordType, hostname, myip);
        return updateSuccess
            ? "ok " + myip
            : "911";
    }

    private static (bool valid, string? login, string? password) GetBasicAuth(string? authorizationHeader)
    {
        if (authorizationHeader == null)
            return (false, null, null);

        try
        {
            var authHeader = AuthenticationHeaderValue.Parse(authorizationHeader);
            if (authHeader.Parameter is not null)
            {
                var credentialString = Encoding.Default.GetString(Convert.FromBase64String(authHeader.Parameter));
                var credentials = credentialString.Split(':', 2);
                return (true, credentials[0], credentials[1]);
            }
        }
        catch { }

        return (false, null, null);
    }
}
