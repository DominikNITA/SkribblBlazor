using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
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

            builder.Services.AddBaseAddressHttpClient();
            builder.Services.AddAuthorizationCore();
            builder.Services.AddTokenAuthenticationStateProvider();
            builder.Services.AddStorage();

            await builder.Build().RunAsync();
        }
    }
}
