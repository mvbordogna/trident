using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using Trident.Contracts.Configuration;
using Trident.UI.Client.Contracts.Models;


namespace Trident.UI.Blazor
{
    public static class ConfigurationExtensions
    {
        public static WebAssemblyHostBuilder ConfigureHttpClients(this WebAssemblyHostBuilder builder, IAppSettings settings)
        {
            try
            {
                var apiConfigs = settings.GetSection<ApiConfiguration[]>("Apis");

                foreach (var apiConfig in apiConfigs)
                {
                    builder.Services.AddHttpClient(apiConfig.Name,
                        client =>
                        {
                            client.BaseAddress = new Uri(apiConfig.BaseUrl);
                        })
                        .AddHttpMessageHandler(sp =>
                        {
                            var x = sp.GetRequiredService<AuthorizationMessageHandler>();
                            x.ConfigureHandler(
                                authorizedUrls: new[]
                                {
                                    apiConfig.BaseUrl,

                                },
                                scopes: new[]
                                {
                                apiConfig.Scopes
                                }
                            );
                            return x;
                        });
                }
            }
            catch (NullReferenceException)
            {
                Console.WriteLine("No Apis Section found");
            }
            return builder;
        }
    }
}
