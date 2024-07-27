namespace CFProxy.API.Dns.CloudFlare.Api;

public record UpdateDnsRecordRequest(
    string Type,
    string Name,
    string Content,
    int TTL,
    bool Proxied)
{ }
