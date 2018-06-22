using System;
using Microsoft.AspNetCore.Builder;

namespace Powerumc.AspNetCore.Mvc.SimpleConfig.Extensions
{
    public static class SimpleConfigMiddlewaresExtension
    {
        public static IApplicationBuilder UseSimpleConfig<TMiddlewaresOptions>(this IApplicationBuilder applicationBuilder,
            Action<TMiddlewaresOptions> optionsBuilder) where TMiddlewaresOptions : SimpleConfigMiddlewaresOptions
        {
            var option = Activator.CreateInstance(typeof(TMiddlewaresOptions), applicationBuilder) as TMiddlewaresOptions;
            optionsBuilder(option);
            
            return applicationBuilder;
        }
    }
}