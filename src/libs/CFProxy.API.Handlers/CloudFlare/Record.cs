namespace CFProxy.API.Handlers.CloudFlare
{
    public class Record
    {
        public string Id { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }

        public static Record FromCloudFlare(Api.DnsRecordsResponse.DnsRecord record)
            => new Record { Id = record.id, Type = record.type, Name = record.name, Value = record.content };
    }
}
