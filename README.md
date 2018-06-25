# Powerumc.AspNetCore.Mvc.SimpleConfig
Simple Configuration in the ASP.NET Core Project

## Install

```
dotnet add package Powerumc.AspNetCore.Mvc.SimpleConfig
```


## Usage

Easily configure frequently used configurations

```csharp
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
            services
                .AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            // Simple configuration.
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
            // Simple configuration.
            app.UseSimpleConfig<SimpleConfigMiddlewaresOptions>(options =>
            {
                options.UseTraceId()
                    .UseCors()
                    .UseEventBus(Path.GetDirectoryName(GetType().Assembly.Location));
            });

            app.UseMvc();
        }
    }
}
```


## See also

- https://github.com/powerumc/Powerumc.AspNetCore.Core
- https://github.com/powerumc/microservice-architecture-quick-start