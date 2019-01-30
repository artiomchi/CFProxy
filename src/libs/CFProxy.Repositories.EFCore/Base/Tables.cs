using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CFProxy.Repositories.EFCore.Base
{
    public class CFAccount
    {
        public int ID { get; set; }
        public string Email { get; set; }
        public string ApiKey { get; set; }

        [InverseProperty(nameof(CFProxyKey.CFAccount))]
        public ICollection<CFProxyKey> CFProxyKeys { get; set; }
        [InverseProperty(nameof(CFZone.CFAccount))]
        public ICollection<CFZone> CFZones { get; set; }
    }

    public class CFProxyKey
    {
        public int ID { get; set; }
        public int CFAccountID { get; set; }
        public string Email { get; set; }
        public string ApiKey { get; set; }

        [ForeignKey(nameof(CFAccountID))]
        public CFAccount CFAccount { get; set; }
        [InverseProperty(nameof(CFProxyKey_Zone.CFProxyKey))]
        public ICollection<CFProxyKey_Zone> CFProxyKey_Zones { get; set; }
    }

    public class CFProxyKey_Zone
    {
        [Key]
        public int CFProxyKeyID { get; set; }
        [Key]
        public int CFZoneID { get; set; }

        [ForeignKey(nameof(CFProxyKeyID))]
        public CFProxyKey CFProxyKey { get; set; }
        [ForeignKey(nameof(CFZoneID))]
        public CFZone CFZone { get; set; }
    }

    public class CFZone
    {
        public int ID { get; set; }
        public int CFAccountID { get; set; }
        public string Domain { get; set; }

        [ForeignKey(nameof(CFAccountID))]
        public CFAccount CFAccount { get; set; }
        [InverseProperty(nameof(CFProxyKey_Zone.CFZone))]
        public ICollection<CFProxyKey_Zone> CFProxyKey_Zones { get; set; }
    }

    public class DynDnsAccount
    {
        public int ID { get; set; }
        public int CFAccountID { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string Host { get; set; }

        [ForeignKey(nameof(CFAccountID))]
        public CFAccount CFAccount { get; set; }
    }
}
