using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Http;

namespace CFProxy.API.Handlers.DynDns
{
    public class NicUpdateHandler
    {
        public Task<string> Process(HttpContext context, CloudFlareClient client)
        {
            var authorizationHeader = context.TryGetHeader("Authorization");
            var userAgentHeader = context.TryGetHeader("User-Agent");
            var myip = (string)context.Request.Query["myip"] ?? context.TryGetRequestIPAddress();
            var hostname = context.Request.Query["hostname"];

            return Process(client, authorizationHeader, userAgentHeader, myip, hostname);
        }

        public Task<string> Process(HttpRequestMessage request, CloudFlareClient client)
        {
            var authorizationHeader = request.TryGetHeader("Authorization");
            var userAgentHeader = request.TryGetHeader("User-Agent");
            var query = HttpUtility.ParseQueryString(request.RequestUri.Query);
            var myip = query["myip"] ?? request.TryGetRequestIPAddress();
            var hostname = query["hostname"];

            return Process(client, authorizationHeader, userAgentHeader, myip, hostname);
        }

        public async Task<string> Process(CloudFlareClient client, string authHeader, string userAgent, string myip, string hostname)
        {
            var credentials = GetBasicAuth(authHeader);
            if (!credentials.valid)
                return "badauth";

            if (string.IsNullOrWhiteSpace(hostname))
                return "nohost";

            client.Authenticate(credentials.login, credentials.password, userAgent);

            var zones = await client.GetZones();
            var zone = zones
                .Where(z => hostname.EndsWith(z.Name, StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(z => z.Name.Length)
                .FirstOrDefault();
            if (zone.Name == null)
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

        private (bool valid, string login, string password) GetBasicAuth(string authorizationHeader)
        {
            if (authorizationHeader == null)
                return (false, null, null);

            try
            {
                var authHeader = AuthenticationHeaderValue.Parse(authorizationHeader);
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
