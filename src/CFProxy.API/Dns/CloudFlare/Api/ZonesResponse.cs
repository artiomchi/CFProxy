namespace CFProxy.API.Dns.CloudFlare.Api;

public record ZonesResponse(bool Success, ZonesResponse.Zone[] Result) : BaseResponse(Success)
{
    public record Zone(string Id, string Name) { }
}
