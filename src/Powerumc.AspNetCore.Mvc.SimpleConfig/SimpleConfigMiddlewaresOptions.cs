using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Powerumc.AspNetCore.Core.Domains;
using Powerumc.AspNetCore.Mvc.SimpleConfig.Middlewares;

namespace Powerumc.AspNetCore.Mvc.SimpleConfig
{
    public class SimpleConfigMiddlewaresOptions
    {
        private readonly IApplicationBuilder _applicationBuilder;

        public SimpleConfigMiddlewaresOptions(IApplicationBuilder applicationBuilder)
        {
            _applicationBuilder = applicationBuilder;
        }

        public SimpleConfigMiddlewaresOptions UseTraceId()
        {
            _applicationBuilder.UseMiddleware<TraceIdMiddleware>();
            
            return this;
        }

        public SimpleConfigMiddlewaresOptions UseCors()
        {
            _applicationBuilder.UseCors("CorsPolicy");

            return this;
        }

        public SimpleConfigMiddlewaresOptions UseEventBus(string path)
        {
            var eventBus = _applicationBuilder.ApplicationServices.GetRequiredService<IEventBus>();
            
            foreach (var filename in Directory.GetFiles(path, "*.dll"))
            {
                var assembly = Assembly.LoadFrom(filename);
                foreach (var assemblyType in assembly.GetTypes())
                {
                    var interfaceTypes = assemblyType
                        .GetInterfaces()
                        .Where(o => o.Name == typeof(IDomainEventHandler<>).Name);

                    foreach (var type in interfaceTypes)
                    {
                        Console.WriteLine($"RegistrationDomainEvent: {type.GetGenericArguments()[0]}");
                        
                        eventBus.Subscribe(type.GetGenericArguments()[0], assemblyType);
                    }
                }
            }

            return this;
        }
    }
}