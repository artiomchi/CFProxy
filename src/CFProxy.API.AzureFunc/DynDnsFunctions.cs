using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using CFProxy.API.Handlers;
using CFProxy.API.Handlers.DynDns;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CFProxy.API.AzureFunc
{
    public static class DynDnsFunctions
    {
        [FunctionName("CheckIP")]
        public static HttpResponseMessage CheckIP(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "checkip")] HttpRequestMessage req)
        {
            var requestIP = req.TryGetRequestIPAddress();
            if (requestIP == null)
            {
                return req.CreateErrorResponse(HttpStatusCode.InternalServerError, "Proxy Failure");
            }

            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(requestIP, Encoding.UTF8, "text/plain");
            return response;
        }

        [FunctionName("NicUpdate")]
        public static async Task<HttpResponseMessage> NicUpdate(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "nic/update")] HttpRequestMessage req)
        {
            using (var scope = Startup.NewScope())
            {
                var handler = scope.ServiceProvider.GetService<NicUpdateHandler>();
                var result = await handler.Process(req);
                var response = req.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(result, Encoding.UTF8, "text/plain");
                return response;
            }
        }
    }
}
