using System.Text.Json.Serialization;
using CFProxy.API.Dns.CloudFlare.Api;

namespace CFProxy.API.Dns.CloudFlare;

[JsonSourceGenerationOptions(WriteIndented = true, PropertyNamingPolicy = JsonKnownNamingPolicy.SnakeCaseLower)]
[JsonSerializable(typeof(CreateDnsRecordRequest))]
[JsonSerializable(typeof(UpdateDnsRecordRequest))]
[JsonSerializable(typeof(BaseResponse))]
[JsonSerializable(typeof(DnsRecordsResponse))]
[JsonSerializable(typeof(ZonesResponse))]
internal partial class CloudFlareJsonGen : JsonSerializerContext
{
}
