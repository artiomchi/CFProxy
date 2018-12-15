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
        private readonly CloudFlareClient _client;

        public NicUpdateHandler(CloudFlareClient client)
        {
            _client = client;
        }

        public Task<string> Process(HttpContext context)
        {
            var authorizationHeader = context.TryGetHeader("Authorization");
            var userAgentHeader = context.TryGetHeader("User-Agent");
            var requestIP = context.TryGetRequestIPAddress();
            var myip = (string)context.Request.Query["myip"] ?? requestIP;
            var hostname = context.Request.Query["hostname"];

            return Process(authorizationHeader, userAgentHeader, requestIP, myip, hostname);
        }

        public Task<string> Process(HttpRequestMessage request)
        {
            var authorizationHeader = request.TryGetHeader("Authorization");
            var userAgentHeader = request.TryGetHeader("User-Agent");
            var query = HttpUtility.ParseQueryString(request.RequestUri.Query);
            var requestIP = request.TryGetRequestIPAddress();
            var myip = query["myip"] ?? requestIP;
            var hostname = query["hostname"];

            return Process(authorizationHeader, userAgentHeader, requestIP, myip, hostname);
        }

        public async Task<string> Process(string authHeader, string userAgent, string requestIP, string myip, string hostname)
        {
            var credentials = GetBasicAuth(authHeader);
            if (!credentials.valid)
                return "badauth";

            if (string.IsNullOrWhiteSpace(hostname))
                return "nohost";

            _client.Authenticate(credentials.login, credentials.password, userAgent, requestIP);

            var zones = await _client.GetZones();
            var zone = zones
                .Where(z => hostname.EndsWith(z.Name, StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(z => z.Name.Length)
                .FirstOrDefault();
            if (zone.Name == null)
                return "911";

            var recordType = myip.IndexOf(':') > 0 ? "AAAA" : "A";

            var records = (await _client.GetRecords(zone.Id, recordType, hostname))
                ?.Where(r => string.Equals(r.Name, hostname, StringComparison.OrdinalIgnoreCase))
                .ToArray();
            if (records == null)
                return "911";
            if (records.Length == 0)
            {
                var createSuccess = await _client.CreateRecord(zone.Id, recordType, hostname, myip);

                return createSuccess
                    ? "ok " + myip
                    : "911";
            }

            foreach (var record in records.Skip(1))
                await _client.DeleteRecord(zone.Id, record.Id);

            var updateSuccess = await _client.UpdateRecord(zone.Id, records[0].Id, recordType, hostname, myip);
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
