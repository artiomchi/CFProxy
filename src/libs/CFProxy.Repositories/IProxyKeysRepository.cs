using System.Collections.Generic;
using System.Threading.Tasks;
using CFProxy.Models;

namespace CFProxy.Repositories
{
    public interface IProxyKeysRepository
    {
        Task<IList<CloudFlareZone>> ListKeyZones(int keyID);
        Task<CloudFlareProxyKey> LoadAsync(string email, string key);
        Task<DynDnsAccount> LoadDynDnsAccount(string login, string password, string host);
    }
}
