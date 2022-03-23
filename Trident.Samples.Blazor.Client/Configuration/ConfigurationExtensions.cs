using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using Trident.Samples.Blazor.Client.Security;

namespace Trident.Samples.Blazor.Client.Configuration
{
    public static class ConfigurationExtensions
    {
        internal static void ConfigureSecurity(this WebAssemblyHostBuilder builder)
        {
            builder.Services.AddSingleton<StateContainer>();

            builder.Services.AddSingleton<StateContainer>();
            //<ApplicationAuthenticationState, CustomUserAccount>
            builder.Services.AddMsalAuthentication(options =>
            {                 
                builder.Configuration.Bind("AzureAdB2C", options.ProviderOptions.Authentication);

                options.ProviderOptions.DefaultAccessTokenScopes.Add("openid");

                options.ProviderOptions.LoginMode = "redirect";
            })
            //.AddAccountClaimsPrincipalFactory<ApplicationAuthenticationState, CustomUserAccount, CustomAccountFactory>()
            ;


            builder.Services.AddAuthorizationCore(config =>
            {
                config.AddPolicy(ClaimsAuthorizeView.PolicyName, policy =>
                {
                    var cr = new ClaimsRequirement();
                    policy.Requirements.Add(cr);
                });

                config.DefaultPolicy = config.GetPolicy(ClaimsAuthorizeView.PolicyName);
            });
        }

        internal static void ConfigureHttpClients(this WebAssemblyHostBuilder builder)
        {
            var apiConfigs = builder.Configuration.GetSection("Apis").Get<ApiConfig[]>();
        
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



    }
}
