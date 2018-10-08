using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Abp.AspNetCore;
using Abp.Castle.Logging.Log4Net;
using Abp.IdentityServer4;
using Castle.Facilities.Logging;
using LTMCompanyNameFree.YoyoCmsTemplate.Authentication.JwtBearer;
using LTMCompanyNameFree.YoyoCmsTemplate.Authorization.Users;
using LTMCompanyNameFree.YoyoCmsTemplate.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ServiceOAuth.Configuration;
using ServiceOAuth.Exs;
using ServiceOAuth.Service.Profiles;
using ServiceOAuth.Validator;

namespace ServiceOAuth
{
    public class Startup
    {
        private readonly IHostingEnvironment _environment;
        private readonly IConfigurationRoot _appConfiguration;

        public Startup(IHostingEnvironment env)
        {
            //Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            _appConfiguration = env.GetAppConfiguration();
            _environment = env;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            string certFile = $"{_environment.ContentRootPath}{Path.DirectorySeparatorChar}Certificates{Path.DirectorySeparatorChar}nanofabrictest.pfx";
            var cert = new X509Certificate2(certFile, "idsrv3test");

            services.AddCors();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

     
            // TODO:IdentiyServer Config Add
            IdentityRegistrar.Register(services);
            //AuthConfigurer.Configure(services, _appConfiguration);

            services.AddIdentityServer()
              .AddDeveloperSigningCredential()
              .AddInMemoryIdentityResources(InMemoryConfiguration.IdentityResources())
              .AddInMemoryApiResources(InMemoryConfiguration.ApiResources())
              .AddInMemoryClients(InMemoryConfiguration.Clients())
              .AddAbpPersistedGrants<IAbpPersistedGrantDbContext>()
              .AddAbpIdentityServer<User>(); ;


            // Configure Abp and Dependency Injection
            return services.AddAbp<IdentityServerModule>(
                // Configure Log4Net logging
                options => options.IocManager.IocContainer.AddFacility<LoggingFacility>(
                    f => f.UseAbpLog4Net().WithConfig("log4net.config")
                )
            );
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseAbp(); // Initializes ABP framework.

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();


            app.UseAuthentication();

            // TODO:IdentiyServer Config Use
            if (bool.Parse(_appConfiguration["Authentication:JwtBearer:IsEnabled"]))
            {
                app.UseJwtTokenMiddleware();
            }
            else if (bool.Parse(_appConfiguration["Authentication:IdentityServer4:IsEnabled"]))
            {
                app.UseJwtTokenMiddleware("IdentityBearer");
                app.UseIdentityServer();
            }



            app.UseMvcWithDefaultRoute();
        }
    }
}
