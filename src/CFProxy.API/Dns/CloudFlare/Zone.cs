namespace CFProxy.API.Dns.CloudFlare;

public record Zone(string Id, string Name)
{
    public Zone(Api.ZonesResponse.Zone zone)
        : this(zone.Id, zone.Name)
    { }
}
