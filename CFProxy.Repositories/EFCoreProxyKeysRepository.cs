using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CFProxy.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace CFProxy.Repositories
{
    public class EFCoreProxyKeysRepository : IProxyKeysRepository
    {
        private readonly CFProxyDbContext _dbContext;

        public EFCoreProxyKeysRepository(CFProxyDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<CFProxyKey> LoadAsync(string email, string key)
            => (await _dbContext.CFProxyKeys
            .Where(c => c.Email == email)
            .Where(c => c.ApiKey == key)
            .ToListAsync())
            .Where(c => c.ApiKey == key) // case sensitive comparison
            .FirstOrDefault();

        public async Task<IList<CFZone>> ListKeyZones(int keyID)
            => await _dbContext.CFProxyKey_Zones
            .Where(kz => kz.CFProxyKeyID == keyID)
            .Select(kz => kz.CFZone)
            .ToListAsync();

        public Task<DynDnsAccount> LoadDynDnsAccount(string login, string password, string host)
            => _dbContext.DynDnsAccounts
            .Include(a => a.CFAccount)
            .Where(a => a.Login == login)
            .Where(a => a.Password == password)
            .Where(a => a.Host == host)
            .FirstOrDefaultAsync();

    }
}
