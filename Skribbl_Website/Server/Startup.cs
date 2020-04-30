using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Skribbl_Website.Server.Hubs;
using Skribbl_Website.Server.Interfaces;
using Skribbl_Website.Server.Services;

namespace Skribbl_Website.Server
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            services.AddSignalR().AddJsonProtocol(options => {
                options.PayloadSerializerOptions.PropertyNamingPolicy = null;
    });
            services.AddSingleton<IWordsProviderService, WordsProviderService>();
            services.AddSingleton<IWordDistanceCalculator, LevenshteinDistance>();
            services.AddTransient<IScoreCalculator, SimpleScoreCalculator>();
            services.AddSingleton<LobbiesManager>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseWebAssemblyDebugging();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseBlazorFrameworkFiles();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                if (env.IsDevelopment())
                {
                    endpoints.MapHub<LobbyHub>("/lobbyHub", options => { options.Transports = HttpTransportType.LongPolling; });
                }
                else
                {
                    endpoints.MapHub<LobbyHub>("/lobbyHub");
                }
                
                endpoints.MapFallbackToFile("index.html");
            });
        }
    }
}
