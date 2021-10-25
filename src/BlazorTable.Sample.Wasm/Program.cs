using BlazorTable.Sample.Shared;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;
using System;

namespace BlazorTable.Sample.Wasm
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            
            builder.RootComponents.Add<App>("app");

            builder.Services.AddSingleton(new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            // To use Bootstrap 4
            builder.Services.AddBlazorTable();

            // To use Bootstrap 5
            //builder.Services.AddBlazorTable(options => options.UseBootstrap5());

            await builder.Build().RunAsync();
        }
    }
}
