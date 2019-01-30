namespace CFProxy.Models
{
    public class CloudFlareProxyKey
    {
        public int ID { get; set; }
        public int CFAccountID { get; set; }
        public string Email { get; set; }
        public string ApiKey { get; set; }

        public CloudFlareAccount CFAccount { get; set; }
    }
}
