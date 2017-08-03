using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using AadUserCreation.Models;
using AadUserCreation.Business.Interfaces;
using AadUserCreation.Business;
using AadUserCreation.Data.Interfaces;
using AadUserCreation.Data;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace AadUserCreation
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            if (env.IsDevelopment())
            {
                // For more details on using the user secret store see https://go.microsoft.com/fwlink/?LinkID=532709
                builder.AddUserSecrets<Startup>();
            }
            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
          
            // Add framework services.
            services.AddMvc();
            services.AddMemoryCache();
            services.AddSession();

            services.AddAuthentication(
                SharedOptions => SharedOptions.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme);

            services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));

            services.AddTransient<IAadUserService, UserSerivice>();
            services.AddTransient<IAadCollector, AadCollectorService>();

            services.AddTransient<IAadCollectieFromAad, AadCollectieFromAad>();
            services.AddTransient<IAadCollectieFromBlob, AadCollectieFromBlob>();

            services.AddTransient<IDepartmentContext, DepartmentContext>();
            services.AddTransient<IGroupContext, GroupContext>();
            services.AddTransient<IWebhookService, WebhookService>();
            services.AddTransient <IBloblogReader, BloblogReader>();
            services.AddTransient<ILogTable, LogTable>();

            services.AddDbContext<AadUserCreationContext>(options =>
                    options.UseSqlServer(Configuration.GetConnectionString("AadUserCreationContext")));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();
            loggerFactory.AddAzureWebAppDiagnostics();


            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseCookieAuthentication();

            app.UseOpenIdConnectAuthentication(new OpenIdConnectOptions
            {
                ClientId = Configuration["Authentication:AzureAd:ClientId"],
                Authority = Configuration["Authentication:AzureAd:AADInstance"] + Configuration["Authentication:AzureAd:TenantId"],
                CallbackPath = Configuration["Authentication:AzureAd:CallbackPath"]
            });

            app.UseSession();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
