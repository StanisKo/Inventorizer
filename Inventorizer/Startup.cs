using System;
using System.Net.Http.Headers;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;

using Npgsql;

using Inventorizer_DataAccess.Data;

using Inventorizer.API.Ebay.Auth;
using Inventorizer.API.Ebay.Provider;
using Inventorizer.API.ForEx;

using Inventorizer.Stats;

namespace Inventorizer
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            NpgsqlConnectionStringBuilder connectionStringBuilder = new NpgsqlConnectionStringBuilder()
            {
                Host = Configuration["Host"],
                Port = Convert.ToInt32(Configuration["Port"]),

                Database = Configuration["Database"],
                Username = Configuration["Username"],
                Password = Configuration["Password"]
            };

            services.AddDbContext<ApplicationDbContext>(
                options => options.UseNpgsql(connectionStringBuilder.ConnectionString)
            );

            services
                .AddSession()
                .AddControllersWithViews()
                .AddSessionStateTempDataProvider()
                .AddRazorRuntimeCompilation();

            services.AddHttpClient("AllPurposeJsonAPI", config =>
            {
                config.Timeout = TimeSpan.FromSeconds(10);
                config.DefaultRequestHeaders.Accept.Clear();
                config.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            });

            /*
            EbayAPIAuthService and ForExAPIService are singletons since
            both of them hold values that must be avaialable for other services
            and with Scoped and Transient those values will be lost

            In addition, spinning up authentication and exchange service with every http request
            puts a strain on performance

            For the same performance considerations EbayAPIProvider and StatsService are also a singletons
            */

            /*
            Registering auth service not through AddHostedService, but as singletone
            to make sure API provider can access auth token from the auth service via DI
            */
            services.AddSingleton<EbayAPIAuthService>();
            services.AddSingleton<IHostedService>(sp => sp.GetService<EbayAPIAuthService>());

            /*
            Doing the same for foreign exchange service, so that Stats
            can access foreign exchange rates via DI
            */
            services.AddSingleton<ForExAPIService>();
            services.AddSingleton<IHostedService>(sp => sp.GetService<ForExAPIService>());

            services.AddSingleton<EbayAPIProvider>();

            services.AddSingleton<StatsService>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseSession();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
