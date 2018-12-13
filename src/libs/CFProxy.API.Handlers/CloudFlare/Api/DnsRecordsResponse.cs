namespace CFProxy.API.Handlers.CloudFlare.Api
{
    public class DnsRecordsResponse : BaseResponse
    {
        public DnsRecord[] result { get; set; }

        public class DnsRecord
        {
            public string id { get; set; }
            public string type { get; set; }
            public string name { get; set; }
            public string content { get; set; }
        }
    }
}
