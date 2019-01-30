using System.Collections.Generic;
using System.Threading.Tasks;
using CFProxy.Repositories.Base;

namespace CFProxy.Repositories
{
    public interface IProxyKeysRepository
    {
        Task<IList<CFZone>> ListKeyZones(int keyID);
        Task<CFProxyKey> LoadAsync(string email, string key);
        Task<DynDnsAccount> LoadDynDnsAccount(string login, string password, string host);
    }
}