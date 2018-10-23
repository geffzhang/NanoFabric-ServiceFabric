using Abp.Application.Services.Dto;
using Abp.Json;
using Abp.MultiTenancy;
using Abp.Web.Models;
using IdentityModel.Client;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace FrontendConsoleApp
{
    class Program
    {
        static string gatwayBaseUrl = "http://localhost:8492";

        static string oauthUrl = "http://localhost:19081/NanoFabric_ServiceFabric/ServiceOAuth";

        static void Main(string[] args)
        {
            RunDemoAsync().Wait();
            Console.ReadLine();
        }



        public static async Task RunDemoAsync()
        {
            var accessToken = await GetAccessTokenViaOwnerPasswordAsync();
            await GetUsersListAsync(accessToken);
            await GetValuesApi(accessToken);
        }


        /// <summary>
        /// get token 
        /// </summary>
        /// <returns></returns>
        private static async Task<string> GetAccessTokenViaOwnerPasswordAsync()
        {

            var httpHandler = new HttpClientHandler();
            httpHandler.CookieContainer.Add(
                //Set TenantId,host is null or empty
                new Uri(oauthUrl), new Cookie(MultiTenancyConsts.TenantIdResolveKey, string.Empty)
            );
            // request token
            var tokenClient = new TokenClient($"{oauthUrl}/connect/token", "52abp.client", "52abpSecret", httpHandler);
            var tokenResponse = await tokenClient.RequestResourceOwnerPasswordAsync("admin", "123qwe");

            if (tokenResponse.IsError)
            {
                Console.WriteLine("Error: ");
                Console.WriteLine(tokenResponse.Error);
            }

            Console.WriteLine(tokenResponse.Json);

            return tokenResponse.AccessToken;
        }

        /// <summary>
        /// get 52abp user list API
        /// </summary>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        private static async Task GetUsersListAsync(string accessToken)
        {
            var client = new HttpClient();
            client.SetBearerToken(accessToken);

            var response = await client.GetAsync($"{gatwayBaseUrl}/test/api/services/app/User/GetAll?SkipCount=0&MaxResultCount=10");
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine(response.StatusCode);
                return;
            }

            var content = await response.Content.ReadAsStringAsync();
            var ajaxResponse = JsonConvert.DeserializeObject<AjaxResponse<PagedResultDto<UserListDto>>>(content);
            if (!ajaxResponse.Success)
            {
                throw new Exception(ajaxResponse.Error?.Message ?? "Remote service throws exception!");
            }

            Console.WriteLine();
            Console.WriteLine("Total user count: " + ajaxResponse.Result.TotalCount);
            Console.WriteLine();
            foreach (var user in ajaxResponse.Result.Items)
            {
                Console.WriteLine($"### UserId: {user.Id}, UserName: {user.UserName}");
                Console.WriteLine(user.ToJsonString(indented: true));
            }
        }

        /// <summary>
        /// get default values API
        /// </summary>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        private static async Task GetValuesApi(string accessToken)
        {
            var httpClient = new HttpClient();
            httpClient.SetBearerToken(accessToken);
            string response = await httpClient.GetStringAsync($"{gatwayBaseUrl}/servicea/api/values")
                .ConfigureAwait(false);
            Console.WriteLine(response);
        }
    }

    internal class UserListDto
    {
        public int Id { get; set; }
        public string UserName { get; set; }
    }
}
