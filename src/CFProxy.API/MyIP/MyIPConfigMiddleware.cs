using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace CFProxy.API.MyIP
{
    public class MyIPConfigMiddleware
    {
        private readonly IConfiguration _configuration;

        public MyIPConfigMiddleware(RequestDelegate _, IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task Invoke(HttpContext context)
        {
            context.Response.ContentType = "application/json";
            var config = JsonConvert.SerializeObject(new
            {
                ipv4Host = _configuration["ipv4Host"],
                ipv6Host = _configuration["ipv6Host"],
            });
            await context.Response.WriteAsync(config);
        }
    }
}
