﻿using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using CFProxy.API.Handlers;
using Microsoft.AspNetCore.Http;

namespace CFProxy.API.CloudFlareProxy
{
    public class CloudFlareProxyMiddleware
    {
        public CloudFlareProxyMiddleware(RequestDelegate _) { }

        public async Task Invoke(HttpContext context, CloudFlareClient client)
        {
            using (var request = new HttpRequestMessage
            {
                Method = new HttpMethod(context.Request.Method),
                RequestUri = new Uri(context.Request.Path.Value.TrimStart('/'), UriKind.Relative),
            })
            {
                var requestIP = context.TryGetRequestIPAddress();
                if (requestIP != null)
                    request.Headers.TryAddWithoutValidation("X-Forwarded-For", requestIP);

                if (context.Request.ContentLength > 0)
                {
                    request.Content = new StreamContent(context.Request.Body);
                    foreach (var header in context.Request.Headers)
                        request.Content.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray());
                }

                using (var result = await client.Client.SendAsync(request))
                {
                    context.Response.StatusCode = (int)result.StatusCode;
                    foreach (var header in result.Headers)
                        context.Response.Headers[header.Key] = header.Value.ToArray();

                    foreach (var header in result.Content.Headers)
                        context.Response.Headers[header.Key] = header.Value.ToArray();

                    context.Response.Headers.Remove("transfer-encoding");
                    using (var responseStream = await result.Content.ReadAsStreamAsync())
                    {
                        await responseStream.CopyToAsync(context.Response.Body);
                    }
                }
            }
        }
    }
}
