﻿using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using CFProxy.API.CloudFlare;
using CFProxy.API.CloudFlare.Api;

namespace CFProxy.API
{
    public class CloudFlareClient
    {
        public CloudFlareClient(HttpClient client)
        {
            Client = client;
        }

        public HttpClient Client { get; }

        public void Authenticate(string email, string key)
        {
            Client.DefaultRequestHeaders.Add("X-Auth-Email", email);
            Client.DefaultRequestHeaders.Add("X-Auth-Key", key);
        }

        public async Task<Zone[]> GetZones()
        {
            var response = await Client.GetAsync("zones");
            if (!response.IsSuccessStatusCode)
                return Array.Empty<Zone>();

            var result = await response.Content.ReadAsAsync<ZonesResponse>();
            if (!result.success)
                return Array.Empty<Zone>();

            return result.result.Select(Zone.FromCloudFlare).ToArray();
        }

        public async Task<Record[]> GetRecords(string zoneid, string type, string name)
        {
            var response = await Client.GetAsync($"zones/{zoneid}/dns_records?name={Uri.EscapeUriString(name)}");
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
            var response = await Client.PostAsJsonAsync($"zones/{zoneid}/dns_records", content);
            if (!response.IsSuccessStatusCode)
                return false;

            var result = await response.Content.ReadAsAsync<BaseResponse>();
            return result.success;
        }

        public async Task<bool> UpdateRecord(string zoneid, string id, string type, string name, string value)
        {
            var content = new UpdateDnsRecordRequest { type = type, name = name, content = value };
            var response = await Client.PutAsJsonAsync($"zones/{zoneid}/dns_records/{id}", content);
            if (!response.IsSuccessStatusCode)
                return false;

            var result = await response.Content.ReadAsAsync<BaseResponse>();
            return result.success;
        }

        public async Task<bool> DeleteRecord(string zoneid, string id)
        {
            var response = await Client.DeleteAsync($"zones/{zoneid}/dns_records/{id}");
            if (!response.IsSuccessStatusCode)
                return false;

            var result = await response.Content.ReadAsAsync<BaseResponse>();
            return result.success;
        }
    }
}
