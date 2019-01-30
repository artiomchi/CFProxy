using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using CFProxy.API.Handlers;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.DependencyInjection;

namespace CFProxy.API.AzureFunc
{
    public static class CloudFlareProxyFunctions
    {
        [FunctionName("CloudFlareProxy")]
        public static async Task<HttpResponseMessage> CloudFlareProxy(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "patch", "post", "put", "delete", Route = "client/v4/{*requestPath}")] HttpRequestMessage req,
            string requestPath)
        {
            using (var scope = Startup.NewScope())
            {
                var cloudFlareClient = scope.ServiceProvider.GetService<CloudFlareClient>();
                var allHeaders = req.Headers.ToDictionary(h => h.Key, h => h.Value);

                Stream requestBody = null;
                if (req.Content != null)
                    requestBody = await req.Content.ReadAsStreamAsync();
                var response = await cloudFlareClient.SendRequest(
                    req.Method,
                    requestPath,
                    req.TryGetRequestIPAddress(),
                    req.GetRequestHeaders(),
                    requestBody);
                return response;
            }
        }
    }
}
