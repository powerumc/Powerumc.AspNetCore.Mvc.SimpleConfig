using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Powerumc.AspNetCore.Core;
using Powerumc.AspNetCore.Core.Domains;
using Powerumc.AspNetCore.Mvc.SimpleConfig.Extensions;

namespace Powerumc.AspNetCore.Mvc.SimpleConfig
{
    public class SimpleConfigOptions
    {
        private readonly IServiceCollection _serviceCollection;
        
        public SimpleConfigOptions(IServiceCollection serviceCollection)
        {
            _serviceCollection = serviceCollection;
        }

        public virtual SimpleConfigOptions AddDefault()
        {
            _serviceCollection.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            return this;
        }

        public virtual SimpleConfigOptions AddDataProtection(string dirFromCurrentDirectory)
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), dirFromCurrentDirectory);
            
            _serviceCollection.AddDataProtection()
                .PersistKeysToFileSystem(new DirectoryInfo(path))
                .DisableAutomaticKeyGeneration();

            return this;
        }

        public virtual SimpleConfigOptions AddCors(string policyName = "CorsPolicy")
        {
            _serviceCollection.AddCors(options => options.AddPolicy(policyName, builder =>
            {
                builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials();
            }));

            return this;
        }
        
        public virtual SimpleConfigOptions AddTraceId()
        {
            _serviceCollection.AddTransient<TraceId>(o =>
            {
                var httpContextAccessor = o.GetRequiredService<IHttpContextAccessor>();
                var traceId = httpContextAccessor.HttpContext.Items["TRACE_ID"];
                if (traceId == null)
                    throw new NullReferenceException(nameof(traceId));

                if (!Guid.TryParse(traceId.ToString(), out var guid))
                    throw new ArgumentNullException(nameof(guid));
                
                return TraceId.New(guid);
            });

            return this;
        }

        public virtual SimpleConfigOptions AddRegistComponents(string dllFilesDirectory)
        {
            foreach (var filename in Directory.GetFiles(dllFilesDirectory, "*.dll"))
            {
                var assembly = Assembly.LoadFrom(filename);
                foreach (var type in assembly.GetTypes())
                {
                    if (!(type.GetCustomAttribute(typeof(RegisterAttribute)) is RegisterAttribute registerAttribute))
                        continue;
                    
                    System.Console.WriteLine($"RegistrationType: {registerAttribute.RegistrationType}, {type}");
                    _serviceCollection.AddSingleton(registerAttribute.RegistrationType, type);
                }
            }
            
            return this;
        }

        public virtual SimpleConfigOptions AddEventBus()
        {
            _serviceCollection.AddSingleton<IEventBus, EventBus>();

            return this;
        }

        public virtual SimpleConfigOptions AddEventBus<TEventBus>() where TEventBus : class, IEventBus
        {
            _serviceCollection.AddSingleton<IEventBus, TEventBus>();

            return this;
        }

        public virtual SimpleConfigOptions AddRegistDomainEventHandler(string dllFilesDirectory)
        {
            foreach (var filename in Directory.GetFiles(dllFilesDirectory, "*.dll"))
            {
                var assembly = Assembly.LoadFrom(filename);
                foreach (var assemblyType in assembly.GetTypes())
                {
                    var interfaceTypes = assemblyType
                        .GetInterfaces()
                        .Where(o => o.Name == typeof(IDomainEventHandler<>).Name);

                    foreach (var type in interfaceTypes)
                    {
                        Console.WriteLine($"RegistrationDomainEventHandler: {assemblyType.Name}");

                        _serviceCollection.AddSingleton(assemblyType);
                    }
                }
            }
            
            return this;
        }
    }
}