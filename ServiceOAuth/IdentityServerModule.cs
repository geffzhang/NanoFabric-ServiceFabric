using Abp.Modules;
using Abp.Reflection.Extensions;
using LTMCompanyNameFree.YoyoCmsTemplate;
using LTMCompanyNameFree.YoyoCmsTemplate.Configuration;
using LTMCompanyNameFree.YoyoCmsTemplate.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceOAuth
{
    [DependsOn(
          typeof(YoyoCmsTemplateWebCoreModule)
  )]
    public class IdentityServerModule : AbpModule
    {
        private readonly IHostingEnvironment _env;
        private readonly IConfigurationRoot _appConfiguration;

        public IdentityServerModule(IHostingEnvironment env)
        {
            _env = env;
            _appConfiguration = env.GetAppConfiguration();
        }

        public override void PreInitialize()
        {
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(IdentityServerModule).GetAssembly());
        }
    }
}
