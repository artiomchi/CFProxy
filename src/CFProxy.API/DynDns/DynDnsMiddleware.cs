using System;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace CFProxy.API.DynDns
{
    public class DynDnsMiddleware
    {
        private readonly RequestDelegate _next;

        public DynDnsMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, CloudFlareClient client)
        {
            var response = await Process(context, client);
            await context.Response.WriteAsync(response);
        }

        public async Task<string> Process(HttpContext context, CloudFlareClient client)
        {
            var credentials = GetBasicAuth(context);
            if (!credentials.valid)
                return "badauth";

            string myip = (string)context.Request.Query["myip"] ?? context.Connection.RemoteIpAddress.ToString();
            string hostname = context.Request.Query["hostname"];
            if (string.IsNullOrWhiteSpace(hostname))
                return "nohost";


            client.Authenticate(credentials.login, credentials.password);

            var zones = await client.GetZones();
            var zone = zones
                .Where(z => hostname.EndsWith(z.Name, StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(z => z.Name.Length)
                .FirstOrDefault();
            if (zone.Name == null)
                return "911";

            var recordType = myip.Contains(':') ? "AAAA" : "A";

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

        private (bool valid, string login, string password) GetBasicAuth(HttpContext context)
        {
            if (!context.Request.Headers.ContainsKey("Authorization"))
                return (false, null, null);

            try
            {
                var authHeader = AuthenticationHeaderValue.Parse(context.Request.Headers["Authorization"]);
                var credentialString = Encoding.UTF8.GetString(Convert.FromBase64String(authHeader.Parameter));
                var credentials = credentialString.Split(new[] { ':' }, 2);
                return (true, credentials[0], credentials[1]);
            }
            catch
            {
                return (false, null, null);
            }
        }
    }
}
