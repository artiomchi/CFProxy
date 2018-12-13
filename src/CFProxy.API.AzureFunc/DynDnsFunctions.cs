using System.Linq;
using System.Net.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace CFProxy.API.AzureFunc
{
    public static class DynDnsFunctions
    {
        [FunctionName("CheckIP")]
        public static string CheckIP(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "checkip")] HttpRequestMessage req)
        {
            string requestIP = req.Headers.GetValues("X-Forwarded-For").FirstOrDefault()?.Split(',').FirstOrDefault();
            return requestIP;
        }
    }
}
