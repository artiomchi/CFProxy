namespace CFProxy.API.CloudFlare.Api
{
    public class ZonesResponse : BaseResponse
    {
        public Zone[] result { get; set; }

        public class Zone
        {
            public string id { get; set; }
            public string name { get; set; }
        }
    }
}
