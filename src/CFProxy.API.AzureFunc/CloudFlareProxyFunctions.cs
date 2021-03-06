using System;
using System.Net.Http;
using System.Threading.Tasks;
using CFProxy.API.Handlers;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

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
            using (var request = new HttpRequestMessage
            {
                Method = req.Method,
                RequestUri = new Uri(requestPath, UriKind.Relative),
            })
            {
                var requestIP = req.TryGetRequestIPAddress();
                if (requestIP != null)
                    request.Headers.TryAddWithoutValidation("X-Forwarded-For", requestIP);

                if (req.Content != null)
                {
                    request.Content = new StreamContent(await req.Content.ReadAsStreamAsync());
                    foreach (var header in req.Content.Headers)
                        request.Content.Headers.TryAddWithoutValidation(header.Key, header.Value);
                }

                var cloudFlareClient = scope.ServiceProvider.GetService<CloudFlareClient>();
                var response = await cloudFlareClient.Client.SendAsync(request);
                return response;
            }
        }
    }
}
