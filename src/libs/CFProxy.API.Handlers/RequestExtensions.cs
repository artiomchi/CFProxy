using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http;

namespace CFProxy.API.Handlers
{
    public static class RequestExtensions
    {
        public static string TryGetHeader(this HttpContext context, string header)
            => context.Request.Headers.ContainsKey(header)
            ? (string)context.Request.Headers[header]
            : null;

        public static string TryGetHeader(this HttpRequestMessage request, string header)
            => request.Headers.TryGetValues(header, out var headerValues)
            ? string.Join(" ", headerValues)
            : null;

        private const string ForwardedForHeader = "X-Forwarded-For";

        public static string TryGetRequestIPAddress(this HttpContext context)
            => context.Request.Headers.ContainsKey(ForwardedForHeader)
            ? context.Request.Headers[ForwardedForHeader].ToArray().FirstOrDefault()?.Split(',').FirstOrDefault()
            : context.Connection.RemoteIpAddress.ToString();

        public static string TryGetRequestIPAddress(this HttpRequestMessage request)
            => request.Headers.TryGetValues(ForwardedForHeader, out var headerValues)
            ? headerValues.FirstOrDefault()?.Split(',').FirstOrDefault()
            : request.Properties.Values.OfType<HttpContext>().FirstOrDefault()?.TryGetRequestIPAddress();

        public static IDictionary<string, ICollection<string>> GetRequestHeaders(this HttpContext context)
        {
            var result = new Dictionary<string, ICollection<string>>(StringComparer.OrdinalIgnoreCase);
            foreach (var header in context.Request.Headers)
                if (result.ContainsKey(header.Key))
                    foreach (var value in header.Value)
                        result[header.Key].Add(value);
                else
                    result[header.Key] = header.Value.ToList();
            return result;
        }

        public static IDictionary<string, ICollection<string>> GetRequestHeaders(this HttpRequestMessage request)
        {
            void AddHeaders(Dictionary<string, ICollection<string>> result, HttpHeaders headers)
            {
                foreach (var header in headers)
                    if (result.ContainsKey(header.Key))
                        foreach (var value in header.Value)
                            result[header.Key].Add(value);
                    else
                        result[header.Key] = header.Value.ToList();
            }

            var dict = new Dictionary<string, ICollection<string>>(StringComparer.OrdinalIgnoreCase);
            AddHeaders(dict, request.Headers);
            if (request.Content != null)
                AddHeaders(dict, request.Content.Headers);
            return dict;
        }
    }
}
