namespace CFProxy.API.CloudFlare
{
    public class Zone
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public static Zone FromCloudFlare(Api.ZonesResponse.Zone zone)
            => new Zone { Id = zone.id, Name = zone.name };
    }
}
