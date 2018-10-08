using System;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Abp.AspNetCore;
using Abp.AspNetCore.Configuration;
using Abp.AspNetCore.SignalR;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Abp.Zero.Configuration;
using LTMCompanyNameFree.YoyoCmsTemplate.Authentication.JwtBearer;
using LTMCompanyNameFree.YoyoCmsTemplate.Configuration;
using LTMCompanyNameFree.YoyoCmsTemplate.EntityFrameworkCore;
using Abp.Configuration.Startup;

namespace LTMCompanyNameFree.YoyoCmsTemplate
{
    [DependsOn(
         typeof(YoyoCmsTemplateApplicationModule),
         typeof(YoyoCmsTemplateEntityFrameworkModule),
         typeof(AbpAspNetCoreModule)
        ,typeof(AbpAspNetCoreSignalRModule)
     )]
    public class YoyoCmsTemplateWebCoreModule : AbpModule
    {
        private readonly IHostingEnvironment _env;
        private readonly IConfigurationRoot _appConfiguration;

        public YoyoCmsTemplateWebCoreModule(IHostingEnvironment env)
        {
            _env = env;
            _appConfiguration = env.GetAppConfiguration();
        }

        public override void PreInitialize()
        {
            Configuration.DefaultNameOrConnectionString = _appConfiguration.GetConnectionString(
                YoyoCmsTemplateConsts.ConnectionStringName
            );

            // TODO:将所有错误信息显示到客户端
            Configuration.Modules.AbpWebCommon().SendAllExceptionsToClients = true;

            // Use database for language management
            Configuration.Modules.Zero().LanguageManagement.EnableDbLocalization();

            Configuration.Modules.AbpAspNetCore()
                 .CreateControllersForAppServices(
                     typeof(YoyoCmsTemplateApplicationModule).GetAssembly()
                 );

            ConfigureTokenAuth();
        }

        private void ConfigureTokenAuth()
        {
            IocManager.Register<TokenAuthConfiguration>();

            // TODO:读取Token配置

            var tokenAuthConfig = IocManager.Resolve<TokenAuthConfiguration>();

            // 默认的配置
            if (bool.Parse(_appConfiguration["Authentication:JwtBearer:IsEnabled"]))
            {
                tokenAuthConfig.SecurityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_appConfiguration["Authentication:JwtBearer:SecurityKey"]));
                tokenAuthConfig.Issuer = _appConfiguration["Authentication:JwtBearer:Issuer"];
                tokenAuthConfig.Audience = _appConfiguration["Authentication:JwtBearer:Audience"];
                tokenAuthConfig.SigningCredentials = new SigningCredentials(tokenAuthConfig.SecurityKey, SecurityAlgorithms.HmacSha256);
                tokenAuthConfig.Expiration = TimeSpan.FromDays(1);

            }// ids4的配置
            else if (bool.Parse(_appConfiguration["Authentication:IdentityServer4:IsEnabled"]))
            {
                tokenAuthConfig.Authority = _appConfiguration["Authentication:IdentityServer4:Authority"];
                tokenAuthConfig.ClientId = _appConfiguration["Authentication:IdentityServer4:ClientId"];
                tokenAuthConfig.Secret = _appConfiguration["Authentication:IdentityServer4:Secret"];
            }

        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(YoyoCmsTemplateWebCoreModule).GetAssembly());
        }
    }
}
