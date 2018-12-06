﻿namespace CFProxy.API.CloudFlare.Api
{
    public class CreateDnsRecordRequest
    {
        public string type { get; set; }
        public string name { get; set; }
        public string content { get; set; }
        public int ttl { get; set; } = 1;
        public bool proxied { get; set; } = false;
    }
}