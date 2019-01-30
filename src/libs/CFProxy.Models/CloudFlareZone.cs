namespace CFProxy.Models
{
    public class CloudFlareZone
    {
        public int ID { get; set; }
        public int CFAccountID { get; set; }
        public string Domain { get; set; }
    }
}
