using Microsoft.EntityFrameworkCore;
using Abp.Zero.EntityFrameworkCore;
using LTMCompanyNameFree.YoyoCmsTemplate.Authorization.Roles;
using LTMCompanyNameFree.YoyoCmsTemplate.Authorization.Users;
using LTMCompanyNameFree.YoyoCmsTemplate.MultiTenancy;
using Abp.IdentityServer4;

namespace LTMCompanyNameFree.YoyoCmsTemplate.EntityFrameworkCore
{
    public class YoyoCmsTemplateDbContext : AbpZeroDbContext<Tenant, Role, User, YoyoCmsTemplateDbContext>
    {
        /* Define a DbSet for each entity of the application */
        //public DbSet<PersistedGrantEntity> PersistedGrants { get; set; }

        public YoyoCmsTemplateDbContext(DbContextOptions<YoyoCmsTemplateDbContext> options)
            : base(options)
        {
        }
    }
}
