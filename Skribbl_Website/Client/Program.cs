using Cloudcrate.AspNetCore.Blazor.Browser.Storage;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Skribbl_Website.Client.Services;
using System.Threading.Tasks;

namespace Skribbl_Website.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");

            builder.Services.AddBaseAddressHttpClient();
            builder.Services.AddStorage();
            builder.Services.AddScoped<UserState>();
            builder.Services.AddSingleton<LobbyConnection>();

            await builder.Build().RunAsync();
        }
    }
}
