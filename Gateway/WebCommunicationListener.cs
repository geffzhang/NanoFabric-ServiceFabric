// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace NanoFabricGateway
{
    using System;
    using System.Fabric;
    using System.Fabric.Description;
    using System.Globalization;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.ServiceFabric.Services.Communication.Runtime;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Configuration;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.Extensions.DependencyInjection;
    using Ocelot.DependencyInjection;
    using Ocelot.Middleware;
    using Ocelot.Cache.CacheManager;
    using Ocelot.Provider.Polly;
    using Ocelot.Administration;
    using IdentityServer4.AccessTokenValidation;

    public class WebCommunicationListener : ICommunicationListener
    {
        private readonly string appRoot;
        private readonly ServiceContext serviceInitializationParameters;
        private string listeningAddress;
        private string publishAddress;

        // OWIN server handle.
        private IWebHost webHost;

        public WebCommunicationListener(string appRoot, ServiceContext serviceInitializationParameters)
        {
            this.appRoot = appRoot;
            this.serviceInitializationParameters = serviceInitializationParameters;
        }

        public Task<string> OpenAsync(CancellationToken cancellationToken)
        {
            ServiceEventSource.Current.Message("Initialize");

            EndpointResourceDescription serviceEndpoint = this.serviceInitializationParameters.CodePackageActivationContext.GetEndpoint("ServiceEndpoint");
            int port = serviceEndpoint.Port;

            this.listeningAddress = string.Format(
                CultureInfo.InvariantCulture,
                "http://+:{0}/{1}",
                port,
                string.IsNullOrWhiteSpace(this.appRoot)
                    ? string.Empty
                    : this.appRoot.TrimEnd('/') + '/');

            this.publishAddress = this.listeningAddress.Replace("+", FabricRuntime.GetNodeContext().IPAddressOrFQDN);

            ServiceEventSource.Current.Message("Starting web server on {0}", this.listeningAddress);

            try
            {
                this.webHost = new WebHostBuilder()
               .UseKestrel()
               .UseUrls(this.listeningAddress)
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config
                        .SetBasePath(hostingContext.HostingEnvironment.ContentRootPath)
                        .AddJsonFile("appsettings.json", true, true)
                        .AddJsonFile($"appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.json", true, true)
                        .AddJsonFile("ocelot.json", false, false)
                        .AddEnvironmentVariables();
                })
               .ConfigureLogging((hostingContext, logging) =>
                {
                    logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                    logging.AddConsole();
                })
                .ConfigureServices(s => {

                    s.AddCors();

                    var authority = "http://localhost:19081/NanoFabric_ServiceFabric/ServiceOAuth";

                    Action<IdentityServerAuthenticationOptions> options = o =>
                    {
                        o.Authority = authority;
                        o.ApiName = "api";
                        o.SupportedTokens = SupportedTokens.Jwt;
                        o.ApiSecret = "secret";
                    };

                    var authenticationProviderKey = "apikey";
                    Action<IdentityServerAuthenticationOptions> options2 = o =>
                    {
                        o.Authority = authority;
                        o.ApiName = "api1";
                        o.SupportedTokens = SupportedTokens.Both;
                        o.RequireHttpsMetadata = false;
                    };

                    s.AddAuthentication()
                    .AddIdentityServerAuthentication(authenticationProviderKey, options2);

                    s.AddOcelot()
                    .AddCacheManager(x =>
                    {
                        x.WithDictionaryHandle();
                    })
                 .AddPolly()
                 .AddAdministration("/administration", options); 

                })
                .Configure(a => {
                    a.UseCors(builder =>
                    {
                        builder.WithOrigins("*");
                        builder.AllowAnyHeader();
                        builder.AllowAnyMethod();
                    });
                    a.UseOcelot().Wait();
                })
               .Build();

                this.webHost.Start();
            }
            catch (Exception ex)
            {
                ServiceEventSource.Current.ServiceWebHostBuilderFailed(ex);
            }

            return Task.FromResult(this.publishAddress);
        }

        public Task CloseAsync(CancellationToken cancellationToken)
        {
            this.StopAll();
            return Task.FromResult(true);
        }

        public void Abort()
        {
            this.StopAll();
        }

        /// <summary>
        /// Stops, cancels, and disposes everything.
        /// </summary>
        private void StopAll()
        {
            try
            {
                if (this.webHost != null)
                {
                    ServiceEventSource.Current.Message("Stopping web server.");
                    this.webHost.Dispose();
                }
            }
            catch (ObjectDisposedException)
            {
            }
        }
    }
}
