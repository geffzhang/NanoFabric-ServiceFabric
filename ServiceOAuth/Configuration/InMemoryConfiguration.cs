using IdentityServer4.Models;
using IdentityServer4.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using static IdentityServer4.IdentityServerConstants;

namespace ServiceOAuth.Configuration
{
    public class InMemoryConfiguration
    {
        public static IEnumerable<ApiResource> ApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource("api1", "ServiceA api")
                {
                    UserClaims = new List<string> { "City", "State" }
                },
                new ApiResource("52abp_admin_api", "52Abp Admin Admin")
            };
        }

        public static IEnumerable<IdentityResource> IdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),            
                new IdentityResources.Profile()
            };
        }

        public static IEnumerable<Client> Clients()
        {
            return new[]
            {
                new Client
                {
                    ClientId = "52abp",
                    ClientSecrets = { new Secret("secret".Sha256()) },
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                    AccessTokenType = AccessTokenType.Jwt,
                    AllowedScopes = {
                        "api1",
                        "52abp_admin_api"
                    },
                    AlwaysSendClientClaims = true
                }
            };
        }

        public static IEnumerable<TestUser> Users()
        {
            return new[]
            {  
                new TestUser
                {
                    SubjectId = "1",
                    Username = "admin",
                    Password = "123qwe",
                    Claims = new List<Claim>
                    {
                        new Claim("City", "Shenzhen"),
                        new Claim("State", "Guangdong")
                    }
                }
            };
        }
    }
}
