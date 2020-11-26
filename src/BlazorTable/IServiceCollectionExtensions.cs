using Microsoft.Extensions.DependencyInjection;

namespace BlazorTable
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddBlazorTable(this IServiceCollection services)
        {
            return services.AddLocalization();
        }
    }
}