using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using System.Net.Http;
using Skribbl_Website.Client.Services;
using Cloudcrate.AspNetCore.Blazor.Browser.Storage;

namespace Skribbl_Website.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");

            //builder.Services.AddBaseAddressHttpClient();
            builder.Services.AddSingleton(new HttpClient {BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)});
            builder.Services.AddStorage();
            builder.Services.AddScoped<UserState>();
            builder.Services.AddSingleton<LobbyConnection>();

            await builder.Build().RunAsync();
        }
    }
}