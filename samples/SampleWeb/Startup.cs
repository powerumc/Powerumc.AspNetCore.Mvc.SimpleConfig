using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Powerumc.AspNetCore.Mvc.SimpleConfig;
using Powerumc.AspNetCore.Mvc.SimpleConfig.Extensions;

namespace SampleWeb
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddSimpleConfig<SimpleConfigOptions>(options =>
            {
                options.AddDefault()
                    .AddDataProtection("../DataProtection-Keys")
                    .AddCors()
                    .AddTraceId()
                    .AddEventBus()
                    .AddRegistComponents(Path.GetDirectoryName(GetType().Assembly.Location))
                    .AddRegistDomainEventHandler(Path.GetDirectoryName(GetType().Assembly.Location));
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseSimpleConfig<SimpleConfigMiddlewaresOptions>(options =>
            {
                options.UseTraceId()
                    .UseCors()
                    .UseEventBus(Path.GetDirectoryName(GetType().Assembly.Location));
            });

            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}