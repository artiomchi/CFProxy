using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using CFProxy.API.Handlers;
using CFProxy.API.Handlers.DynDns;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace CFProxy.API.AzureFunc
{
    public static class DynDnsFunctions
    {
        [FunctionName("CheckIP")]
        public static HttpResponseMessage CheckIP(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "checkip")] HttpRequestMessage req,
            ILogger logger)
        {
            var requestIP = req.TryGetRequestIPAddress();
            if (requestIP == null)
            {
                logger.LogError("Request not proxied, can't get the request IP address");
                return req.CreateErrorResponse(HttpStatusCode.InternalServerError, "Proxy Failure");
            }
            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(requestIP, Encoding.UTF8, "text/plain");
            return response;
        }

        [FunctionName("NicUpdate")]
        public static async Task<HttpResponseMessage> NicUpdate(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "nic/update")] HttpRequestMessage req,
            ILogger logger)
        {
            var handler = new NicUpdateHandler();
            var cloudFlareClient = new CloudFlareClient(CloudFlareProxyFunctions.Client);
            var result = await handler.Process(req, cloudFlareClient);
            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(result, Encoding.UTF8, "text/plain");
            return response;
        }
    }
}
