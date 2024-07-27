namespace CFProxy.API.Dns.CloudFlare.Api;

public record CreateDnsRecordRequest(
    string Type,
    string Name,
    string Content,
    int TTL,
    bool Proxied)
{ }
