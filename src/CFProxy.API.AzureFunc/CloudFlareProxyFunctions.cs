using System;
using System.Net.Http;
using System.Threading.Tasks;
using CFProxy.API.Handlers;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace CFProxy.API.AzureFunc
{
    public static class CloudFlareProxyFunctions
    {
        static CloudFlareProxyFunctions()
        {
            Client = new HttpClient()
            {
                BaseAddress = new Uri("https://api.cloudflare.com/client/v4/"),
                DefaultRequestHeaders = { { "X-Proxy-User-Agent", "CFProxy (+https://cfproxy.com)" } },
            };
        }

        public static HttpClient Client { get; private set; }

        [FunctionName("CloudFlareProxy")]
        public static async Task<HttpResponseMessage> CloudFlareProxy(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "patch", "post", "put", "delete", Route = "client/v4/{*requestPath}")] HttpRequestMessage req,
            string requestPath,
            ILogger log)
        {
            var cloudFlareClient = new CloudFlareClient(Client);

            using (var request = new HttpRequestMessage
            {
                Method = req.Method,
                RequestUri = new Uri(requestPath, UriKind.Relative),
            })
            {
                if (req.Content != null)
                    request.Content = new StreamContent(await req.Content.ReadAsStreamAsync());

                foreach (var header in req.Headers)
                    if (!string.Equals(header.Key, "host", StringComparison.OrdinalIgnoreCase))
                        if (!request.Headers.TryAddWithoutValidation(header.Key, header.Value) && request.Content != null)
                            request.Content.Headers.TryAddWithoutValidation(header.Key, header.Value);

                var response = await cloudFlareClient.Client.SendAsync(request);
                return response;
            }
        }
    }
}
