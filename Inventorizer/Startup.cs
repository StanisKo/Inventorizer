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

            services.AddControllersWithViews().AddRazorRuntimeCompilation();

            services.AddHttpClient("AllPurposeJsonAPI", config =>
            {
                // No BaseAddress since the client is created for different services ...
                config.Timeout = TimeSpan.FromSeconds(10);
                config.DefaultRequestHeaders.Accept.Clear();
                config.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            });

            /*
            Registering auth service not through AddHostedService, but as singletone
            to make sure API provider can access auth token from the auth service via DI
            */
            services.AddSingleton<EbayAPIAuthService>();
            services.AddSingleton<IHostedService>(sp => sp.GetService<EbayAPIAuthService>());

            services.AddSingleton<EbayAPIProvider>();
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

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
