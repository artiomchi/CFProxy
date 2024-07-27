using CFProxy.API.Dns.CloudFlare;
using CFProxy.API.Dns.CloudFlare.Api;

namespace CFProxy.API.Dns;

public class CloudFlareDnsClient(HttpClient client) : IDnsClient
{
    public HttpClient Client { get; } = client;

    public void Authenticate(string email, string key, string? userAgent, string? requestIP)
    {
        if (!string.IsNullOrEmpty(userAgent))
            Client.DefaultRequestHeaders.Add("User-Agent", userAgent);
        if (!string.IsNullOrEmpty(requestIP))
            Client.DefaultRequestHeaders.Add("X-Forwarded-For", requestIP);
        Client.DefaultRequestHeaders.Add("X-Auth-Email", email);
        Client.DefaultRequestHeaders.Add("X-Auth-Key", key);
    }

    public async Task<Zone[]> GetZones()
    {
        var response = await Client.GetAsync("zones");
        if (!response.IsSuccessStatusCode)
            return [];

        var result = await response.Content.ReadFromJsonAsync(CloudFlareJsonGen.Default.ZonesResponse);
        if (result?.Success != true)
            return [];

        return result.Result.Select(z => new Zone(z)).ToArray();
    }

    public async Task<Record[]> GetRecords(string zoneid, string type, string name)
    {
        var response = await Client.GetAsync($"zones/{zoneid}/dns_records?name={Uri.EscapeDataString(name)}");
        if (!response.IsSuccessStatusCode)
            return [];

        var result = await response.Content.ReadFromJsonAsync(CloudFlareJsonGen.Default.DnsRecordsResponse);
        if (result?.Success != true)
            return [];

        return result.Result.Select(r => new Record(r)).ToArray();
    }

    public async Task<bool> CreateRecord(string zoneid, string type, string name, string value)
    {
        var content = new CreateDnsRecordRequest(type, name, value, 0, false);
        var response = await Client.PostAsJsonAsync($"zones/{zoneid}/dns_records", content, CloudFlareJsonGen.Default.CreateDnsRecordRequest);
        if (!response.IsSuccessStatusCode)
            return false;

        var result = await response.Content.ReadFromJsonAsync(CloudFlareJsonGen.Default.BaseResponse);
        return result?.Success ?? false;
    }

    public async Task<bool> UpdateRecord(string zoneid, string id, string type, string name, string value)
    {
        var content = new UpdateDnsRecordRequest(type, name, value, 0, false);
        var response = await Client.PutAsJsonAsync($"zones/{zoneid}/dns_records/{id}", content, CloudFlareJsonGen.Default.UpdateDnsRecordRequest);
        if (!response.IsSuccessStatusCode)
            return false;

        var result = await response.Content.ReadFromJsonAsync(CloudFlareJsonGen.Default.BaseResponse);
        return result?.Success ?? false;
    }

    public async Task<bool> DeleteRecord(string zoneid, string id)
    {
        var response = await Client.DeleteAsync($"zones/{zoneid}/dns_records/{id}");
        if (!response.IsSuccessStatusCode)
            return false;

        var result = await response.Content.ReadFromJsonAsync(CloudFlareJsonGen.Default.BaseResponse);
        return result?.Success ?? false;
    }
}
