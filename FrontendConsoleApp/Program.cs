using IdentityModel.Client;
using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace FrontendConsoleApp
{
    class Program
    {
        static string baseUrl = "http://127.0.0.1:8492";
        static void Main(string[] args)
        {
            A();
            var tokenClient = new TokenClient($"{baseUrl}/serviceoauth/connect/token", "52abp", "secret");
            var tokenResponse = tokenClient.RequestResourceOwnerPasswordAsync("bob@weyhd.com", "bob123!", "api1").ConfigureAwait(false)
                .GetAwaiter().GetResult();

            if (tokenResponse.IsError)
            {
                throw new ApplicationException($"Status code: {tokenResponse.IsError}, Error: {tokenResponse.Error}");
            }

            var httpClient = new HttpClient();
            httpClient.SetBearerToken(tokenResponse.AccessToken);
            for (int i = 0; i < 10; i++)
            {
                string response = httpClient.GetStringAsync($"{baseUrl}/servicea/api/values").ConfigureAwait(false).GetAwaiter().GetResult();
                Console.WriteLine(response);
            }
            Console.ReadLine();

        }


        static void A()
        {
            var httpClient = new HttpClient();
            string response = httpClient.GetStringAsync($"{baseUrl}/test/AbpUserConfiguration/GetAll").ConfigureAwait(false).GetAwaiter().GetResult();
        }
    }
}
