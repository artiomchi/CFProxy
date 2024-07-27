using CFProxy.API.Dns.CloudFlare;

namespace CFProxy.API.Dns;

public interface IDnsClient
{
    HttpClient Client { get; }

    void Authenticate(string email, string key, string? userAgent, string? requestIP);
    Task<bool> CreateRecord(string zoneid, string type, string name, string value);
    Task<bool> DeleteRecord(string zoneid, string id);
    Task<Record[]> GetRecords(string zoneid, string type, string name);
    Task<Zone[]> GetZones();
    Task<bool> UpdateRecord(string zoneid, string id, string type, string name, string value);
}
