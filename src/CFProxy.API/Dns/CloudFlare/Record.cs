namespace CFProxy.API.Dns.CloudFlare;

public record Record(
    string Id,
    string Type,
    string Name,
    string Value)
{
    public Record(Api.DnsRecordsResponse.DnsRecord record)
        : this(record.Id, record.Type, record.Name, record.Content)
    { }
}
