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
        static string baseUrl = "http://127.0.0.1:8492";
        static void Main(string[] args)
        {
            RunDemoAsync().Wait();
            Console.ReadLine();
            
            //var tokenClient = new TokenClient($"{baseUrl}/serviceoauth/connect/token", "52abp", "secret");
            //var tokenResponse = tokenClient.RequestResourceOwnerPasswordAsync("admin", "123qwe").ConfigureAwait(false)
            //    .GetAwaiter().GetResult();

            //if (tokenResponse.IsError)
            //{
            //    throw new ApplicationException($"Status code: {tokenResponse.IsError}, Error: {tokenResponse.Error}");
            //}

            //var httpClient = new HttpClient();
            //httpClient.SetBearerToken(tokenResponse.AccessToken);
            //for (int i = 0; i < 10; i++)
            //{
            //    string response = httpClient.GetStringAsync($"{baseUrl}/servicea/api/values").ConfigureAwait(false).GetAwaiter().GetResult();
            //    Console.WriteLine(response);
            //}
            //Console.ReadLine();

        }



        public static async Task RunDemoAsync()
        {
            var accessToken = await GetAccessTokenViaOwnerPasswordAsync();
            await GetUsersListAsync(accessToken);
        }

        private static async Task<string> GetAccessTokenViaOwnerPasswordAsync()
        {
            var disco = await DiscoveryClient.GetAsync("http://localhost:8720");

            var httpHandler = new HttpClientHandler();
            httpHandler.CookieContainer.Add(new Uri("http://localhost:8720/"), new Cookie(MultiTenancyConsts.TenantIdResolveKey, "1")); //Set TenantId
            var tokenClient = new TokenClient(disco.TokenEndpoint, "client", "secret", httpHandler);
            var tokenResponse = await tokenClient.RequestResourceOwnerPasswordAsync("admin", "123qwe");

            if (tokenResponse.IsError)
            {
                Console.WriteLine("Error: ");
                Console.WriteLine(tokenResponse.Error);
            }

            Console.WriteLine(tokenResponse.Json);

            return tokenResponse.AccessToken;
        }

        private static async Task GetUsersListAsync(string accessToken)
        {
            var client = new HttpClient();
            client.SetBearerToken(accessToken);

            var response = await client.GetAsync("http://localhost:8492/test/api/services/app/Role/GetAll?SkipCount=0&MaxResultCount=10");
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
    }

    internal class UserListDto
    {
        public int Id { get; set; }
        public string UserName { get; set; }
    }
}
