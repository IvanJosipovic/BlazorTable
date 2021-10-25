using Microsoft.Extensions.DependencyInjection;
using System;

namespace BlazorTable
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddBlazorTable(this IServiceCollection services, Action<BlazorTableOptions> configureOptions = default)
        {
            if (configureOptions != default)
            {
                services.Configure(configureOptions);
            }
            return services.AddLocalization();
        }
    }
}