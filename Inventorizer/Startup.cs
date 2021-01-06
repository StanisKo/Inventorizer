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
using Inventorizer.API;

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
            /*
            We could've stored connection string in appsettings.json,
            but to avoid having db credentials in the version control
            we store them in dotnet user-secrets package:

            https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-5.0&tabs=linux
            */
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

            services.AddHttpClient("EbayAPI", config =>
            {
                config.BaseAddress = new Uri(Configuration.GetValue<string>("EbayAPI:Base"));
                config.DefaultRequestHeaders.Accept.Clear();
                config.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            });

            // Add Ebay API to DI chain
            services.AddSingleton<EbayAPI>();
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
