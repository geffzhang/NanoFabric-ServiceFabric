using IdentityModel.Client;
using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace FrontendConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {

            var tokenClient = new TokenClient("http://localhost:8492/serviceoauth/connect/token", "52abp", "secret");
            var tokenResponse = tokenClient.RequestResourceOwnerPasswordAsync("bob@weyhd.com", "bob123!", "api1").ConfigureAwait(false)
                .GetAwaiter().GetResult();

            if (tokenResponse.IsError)
            {
                throw new ApplicationException($"Status code: {tokenResponse.IsError}, Error: {tokenResponse.Error}");
            }

            var httpClient = new HttpClient();
            httpClient.SetBearerToken(tokenResponse.AccessToken);

            string response = httpClient.GetStringAsync($"http://localhost:8492/servicea/api/values").ConfigureAwait(false).GetAwaiter().GetResult();
            Console.WriteLine(response);
            Console.ReadLine();

        }
    }
}
