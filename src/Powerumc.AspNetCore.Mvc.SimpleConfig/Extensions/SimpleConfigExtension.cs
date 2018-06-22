using System;
using Microsoft.Extensions.DependencyInjection;

namespace Powerumc.AspNetCore.Mvc.SimpleConfig.Extensions
{
    public static class SimpleConfigExtension
    {
        public static IServiceCollection AddSimpleConfig<TOptions>(this IServiceCollection serviceCollection,
            Action<TOptions> optionsBuilder) where TOptions : SimpleConfigOptions
        {
            var option = Activator.CreateInstance(typeof(TOptions), serviceCollection) as TOptions;
            optionsBuilder(option);

            return serviceCollection;
        }
    }
}