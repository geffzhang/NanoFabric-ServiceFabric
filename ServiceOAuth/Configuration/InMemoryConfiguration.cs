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
                }
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
                    AllowedGrantTypes = GrantTypes.HybridAndClientCredentials,
                    AccessTokenType = AccessTokenType.Jwt,
                    //RedirectUris = { "http://localhost:9000/signin-oidc" },
                    AllowedScopes = {
                        StandardScopes.OpenId,
                        StandardScopes.Profile,
                        StandardScopes.OfflineAccess,
                        "api1"
                    }
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
                    Username = "bob@weyhd.com",
                    Password = "bob123!",
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
