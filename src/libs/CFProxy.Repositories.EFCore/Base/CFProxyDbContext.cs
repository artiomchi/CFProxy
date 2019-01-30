using Microsoft.EntityFrameworkCore;

namespace CFProxy.Repositories.EFCore.Base
{
    public class CFProxyDbContext : DbContext
    {
        public DbSet<CFAccount> CFAccounts { get; set; }
        public DbSet<CFProxyKey> CFProxyKeys { get; set; }
        public DbSet<CFProxyKey_Zone> CFProxyKey_Zones { get; set; }
        public DbSet<CFZone> CFZones { get; set; }
        public DbSet<DynDnsAccount> DynDnsAccounts { get; set; }
    }
}
