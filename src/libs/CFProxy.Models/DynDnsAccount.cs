namespace CFProxy.Models
{
    public class DynDnsAccount
    {
        public int ID { get; set; }
        public int CFAccountID { get; set; }
        public string Host { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }

        public CloudFlareAccount CFAccount { get; set; }
    }
}
