using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using CFProxy.API.Handlers.CloudFlare;
using CFProxy.API.Handlers.CloudFlare.Api;
using CFProxy.Repositories;

namespace CFProxy.API.Handlers
{
    public class CloudFlareClient
    {
        private readonly HttpClient _client;
        private readonly IProxyKeysRepository _proxyKeysRepository;

        public CloudFlareClient(HttpClient client, IProxyKeysRepository proxyKeysRepository)
        {
            _client = client;
            _proxyKeysRepository = proxyKeysRepository;
        }

        public static void ConfigureClient(HttpClient client)
        {
            client.BaseAddress = new Uri("https://api.cloudflare.com/client/v4/");
            client.DefaultRequestHeaders.Add("X-Proxy-User-Agent", "CFProxy (+https://cfproxy.com)");
        }

        public void Authenticate(string email, string key, string userAgent, string requestIP)
        {
            if (!string.IsNullOrEmpty(userAgent))
                _client.DefaultRequestHeaders.Add("User-Agent", userAgent);
            if (!string.IsNullOrEmpty(requestIP))
                _client.DefaultRequestHeaders.Add("X-Forwarded-For", requestIP);
            _client.DefaultRequestHeaders.Add("X-Auth-Email", email);
            _client.DefaultRequestHeaders.Add("X-Auth-Key", key);
        }

        public async Task<HttpResponseMessage> SendRequest(HttpMethod method, string requestPath, string requestIP, IDictionary<string, ICollection<string>> requestHeaders, Stream requestBody)
        {
            using (var request = new HttpRequestMessage
            {
                Method = method,
                RequestUri = new Uri(requestPath, UriKind.Relative),
            })
            {
                if (requestHeaders.TryGetValue("X-Auth-Email", out var authEmails) && requestHeaders.TryGetValue("X-Auth-Key", out var authKeys))
                {
                    var proxyKey = await _proxyKeysRepository.LoadAsync(authEmails.FirstOrDefault(), authKeys.FirstOrDefault());
                    if (proxyKey != null)
                    {
                        request.Headers.TryAddWithoutValidation("X-Auth-Email", proxyKey.CFAccount.Email);
                        request.Headers.TryAddWithoutValidation("X-Auth-Key", proxyKey.CFAccount.ApiKey);
                    }
                }

                if (requestIP != null)
                    request.Headers.TryAddWithoutValidation("X-Forwarded-For", requestIP);

                if (requestBody != null)
                {
                    request.Content = new StreamContent(requestBody);
                    foreach (var header in requestHeaders.Where(h => h.Key.StartsWith("Content-", StringComparison.OrdinalIgnoreCase)))
                        request.Content.Headers.TryAddWithoutValidation(header.Key, header.Value);
                }

                return await _client.SendAsync(request);
            }
        }

        public async Task<Zone[]> GetZones()
        {
            var response = await _client.GetAsync("zones");
            if (!response.IsSuccessStatusCode)
                return Array.Empty<Zone>();

            var result = await response.Content.ReadAsAsync<ZonesResponse>();
            if (!result.success)
                return Array.Empty<Zone>();

            return result.result.Select(Zone.FromCloudFlare).ToArray();
        }

        public async Task<Record[]> GetRecords(string zoneid, string type, string name)
        {
            var response = await _client.GetAsync($"zones/{zoneid}/dns_records?name={Uri.EscapeUriString(name)}");
            if (!response.IsSuccessStatusCode)
                return Array.Empty<Record>();

            var result = await response.Content.ReadAsAsync<DnsRecordsResponse>();
            if (!result.success)
                return Array.Empty<Record>();

            return result.result.Select(Record.FromCloudFlare).ToArray();
        }

        public async Task<bool> CreateRecord(string zoneid, string type, string name, string value)
        {
            var content = new CreateDnsRecordRequest { type = type, name = name, content = value };
            var response = await _client.PostAsJsonAsync($"zones/{zoneid}/dns_records", content);
            if (!response.IsSuccessStatusCode)
                return false;

            var result = await response.Content.ReadAsAsync<BaseResponse>();
            return result.success;
        }

        public async Task<bool> UpdateRecord(string zoneid, string id, string type, string name, string value)
        {
            var content = new UpdateDnsRecordRequest { type = type, name = name, content = value };
            var response = await _client.PutAsJsonAsync($"zones/{zoneid}/dns_records/{id}", content);
            if (!response.IsSuccessStatusCode)
                return false;

            var result = await response.Content.ReadAsAsync<BaseResponse>();
            return result.success;
        }

        public async Task<bool> DeleteRecord(string zoneid, string id)
        {
            var response = await _client.DeleteAsync($"zones/{zoneid}/dns_records/{id}");
            if (!response.IsSuccessStatusCode)
                return false;

            var result = await response.Content.ReadAsAsync<BaseResponse>();
            return result.success;
        }
    }
}
