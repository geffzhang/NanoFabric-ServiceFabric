using Abp.Authorization;
using Abp.Authorization.Users;
using Abp.Localization;
using Abp.MultiTenancy;
using Abp.Runtime.Session;
using Abp.UI;
using AutoMapper.Configuration;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using LTMCompanyNameFree.YoyoCmsTemplate.Authentication.External;
using LTMCompanyNameFree.YoyoCmsTemplate.Authentication.JwtBearer;
using LTMCompanyNameFree.YoyoCmsTemplate.Authorization;
using LTMCompanyNameFree.YoyoCmsTemplate.Authorization.Users;
using LTMCompanyNameFree.YoyoCmsTemplate.MultiTenancy;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ServiceOAuth.Validator
{
    public class ResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
    {
        public IAbpSession AbpSession { get; set; }
        private readonly IConfiguration _appConfiguration;
        private readonly LogInManager _logInManager;
        private readonly ITenantCache _tenantCache;
        private readonly AbpLoginResultTypeHelper _abpLoginResultTypeHelper;
        private readonly TokenAuthConfiguration _configuration;
        private readonly IExternalAuthConfiguration _externalAuthConfiguration;
        private readonly IExternalAuthManager _externalAuthManager;
        private readonly UserRegistrationManager _userRegistrationManager;


        public ResourceOwnerPasswordValidator(
            IConfiguration appConfiguration,
            LogInManager logInManager,
            ITenantCache tenantCache,

            UserRegistrationManager userRegistrationManager)
        {
            AbpSession = NullAbpSession.Instance;
        }

        public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            try
            {
                var loginResult = await GetLoginResultAsync(
                         context.UserName,
                         context.Password,
                         GetTenancyNameOrNull()
                     );

                context.Result = new GrantValidationResult(
                    subject: context.UserName,
                    authenticationMethod: "custom",
                    claims: CreateJwtClaims(loginResult.Identity)
                    );
            }
            catch (Exception e)
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, e.Message);
            }

            ////根据context.UserName和context.Password与数据库的数据做校验，判断是否合法
            //if (context.UserName == "admin" && context.Password == "123")
            //{
            //    context.Result = new GrantValidationResult(
            //        subject: context.UserName,
            //        authenticationMethod: "custom",
            //        claims: new Claim[]
            //        {
            //            new Claim("Name", context.UserName),

            //            new Claim("UserId", "1"),
            //            new Claim("RealName", "玩双截棍的熊猫"),
            //            new Claim("Email", "msmadaoe@msn.com")
            //        });
            //}
            //else
            //{
            //    //验证失败
            //    context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "invalid custom credential");
            //}
        }

        private async Task<AbpLoginResult<Tenant, User>> GetLoginResultAsync(string usernameOrEmailAddress, string password, string tenancyName)
        {
            var loginResult = await _logInManager.LoginAsync(usernameOrEmailAddress, password, tenancyName);

            switch (loginResult.Result)
            {
                case AbpLoginResultType.Success:
                    return loginResult;
                default:
                    throw new UserFriendlyException("Login Failed");
            }
        }

        private string GetTenancyNameOrNull()
        {
            if (!AbpSession.TenantId.HasValue)
            {
                return null;
            }

            return _tenantCache.GetOrNull(AbpSession.TenantId.Value)?.TenancyName;
        }

        //private Exception CreateExceptionForFailedLoginAttempt(AbpLoginResultType result, string usernameOrEmailAddress, string tenancyName)
        //{
        //    //switch (result)
        //    //{
        //    //    case AbpLoginResultType.Success:
        //    //        return new Exception("Don't call this method with a success result!");
        //    //    case AbpLoginResultType.InvalidUserNameOrEmailAddress:
        //    //    case AbpLoginResultType.InvalidPassword:
        //    //        return new UserFriendlyException(L("LoginFailed"), L("InvalidUserNameOrPassword"));
        //    //    case AbpLoginResultType.InvalidTenancyName:
        //    //        return new UserFriendlyException(L("LoginFailed"), L("ThereIsNoTenantDefinedWithName{0}", tenancyName));
        //    //    case AbpLoginResultType.TenantIsNotActive:
        //    //        return new UserFriendlyException(L("LoginFailed"), L("TenantIsNotActive", tenancyName));
        //    //    case AbpLoginResultType.UserIsNotActive:
        //    //        return new UserFriendlyException(L("LoginFailed"), L("UserIsNotActiveAndCanNotLogin", usernameOrEmailAddress));
        //    //    case AbpLoginResultType.UserEmailIsNotConfirmed:
        //    //        return new UserFriendlyException(L("LoginFailed"), L("UserEmailIsNotConfirmedAndCanNotLogin"));
        //    //    case AbpLoginResultType.LockedOut:
        //    //        return new UserFriendlyException(L("LoginFailed"), L("UserLockedOutMessage"));
        //    //    default: // Can not fall to default actually. But other result types can be added in the future and we may forget to handle it
        //    //        //Logger.Warn("Unhandled login fail reason: " + result);
        //    //        return new UserFriendlyException("LoginFailed");
        //    //}
        //}


        private static List<Claim> CreateJwtClaims(ClaimsIdentity identity)
        {
            var claims = identity.Claims.ToList();
            var nameIdClaim = claims.First(c => c.Type == ClaimTypes.NameIdentifier);

            // Specifically add the jti (random nonce), iat (issued timestamp), and sub (subject/user) claims.
            claims.AddRange(new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, nameIdClaim.Value),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                //new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.Now.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
            });

            return claims;
        }
    }
}
