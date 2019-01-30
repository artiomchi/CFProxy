using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CFProxy.Models;
using Microsoft.EntityFrameworkCore;
using Entities = CFProxy.Repositories.EFCore.Base;

namespace CFProxy.Repositories.EFCore
{
    public class EFCoreProxyKeysRepository : IProxyKeysRepository
    {
        private readonly Entities.CFProxyDbContext _dbContext;

        public EFCoreProxyKeysRepository(Entities.CFProxyDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<CloudFlareProxyKey> LoadAsync(string email, string key)
            => (await _dbContext.CFProxyKeys
            .Where(c => c.Email == email)
            .Where(c => c.ApiKey == key)
            .ToListAsync())
            .Where(c => c.ApiKey == key) // case sensitive comparison
            .Select(c => new CloudFlareProxyKey
            {
                ID = c.ID,
                CFAccountID = c.CFAccountID,
                Email = c.Email,
                ApiKey = c.ApiKey,
                CFAccount = new CloudFlareAccount
                {
                    ID = c.CFAccount.ID,
                    Email = c.CFAccount.Email,
                    ApiKey = c.CFAccount.ApiKey,
                },
            })
            .FirstOrDefault();

        public async Task<IList<CloudFlareZone>> ListKeyZones(int keyID)
            => await _dbContext.CFProxyKey_Zones
            .Where(kz => kz.CFProxyKeyID == keyID)
            .Select(kz => kz.CFZone)
            .Select(kz => new CloudFlareZone
            {
                ID = kz.ID,
                CFAccountID = kz.CFAccountID,
                Domain = kz.Domain,
            })
            .ToListAsync();

        public Task<DynDnsAccount> LoadDynDnsAccount(string login, string password, string host)
            => _dbContext.DynDnsAccounts
            .Include(a => a.CFAccount)
            .Where(a => a.Login == login)
            .Where(a => a.Password == password)
            .Where(a => a.Host == host)
            .Select(a => new DynDnsAccount
            {
                ID = a.ID,
                CFAccountID = a.CFAccountID,
                Host = a.Host,
                Login = a.Login,
                Password = a.Password,
                CFAccount = new CloudFlareAccount
                {
                    ID = a.CFAccount.ID,
                    Email = a.CFAccount.Email,
                    ApiKey = a.CFAccount.ApiKey,
                },
            })
            .FirstOrDefaultAsync();

    }
}
