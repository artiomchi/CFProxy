namespace CFProxy.API.Dns.CloudFlare.Api;

public record DnsRecordsResponse(bool Success, DnsRecordsResponse.DnsRecord[] Result) : BaseResponse(Success)
{
    public record DnsRecord(
        string Id,
        string Type,
        string Name,
        string Content)
    { }
}
