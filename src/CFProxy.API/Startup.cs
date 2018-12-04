using System;
using CFProxy.API.DynDns;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CFProxy.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHsts(options =>
            {
                options.MaxAge = TimeSpan.FromDays(730);
            });
            services.AddHttpsRedirection(options =>
            {
                options.HttpsPort = 443;
            });

            services.AddHttpClient<CloudFlareClient>(client =>
            {
                client.BaseAddress = new Uri("https://api.cloudflare.com/client/v4/");
                client.DefaultRequestHeaders.Add("User-Agent", "CFProxy (+https://cfproxy.com)");
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseDynDnsHandler();
        }
    }
}
