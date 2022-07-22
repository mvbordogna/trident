using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using Trident.Contracts.Configuration;
using Trident.UI.Blazor.Components.Security;


namespace Trident.UI.Blazor.Security
{
    public static class SecurityExtensions
    {
        public static WebAssemblyHostBuilder ConfigureSecurity(this WebAssemblyHostBuilder builder, IAppSettings settings)
        {
            builder.Services.AddSingleton<StateContainer>();
            builder.ConfigureOkta(settings);
            builder.ConfigureAzureB2C(settings);
            return builder.ConfigureAppAuthorization(settings);
        }

        internal static WebAssemblyHostBuilder ConfigureOkta(this WebAssemblyHostBuilder builder, IAppSettings settings)
        {
            try
            {
                var oktaSettings = settings.GetSection<OktaSettings>("Okta");

                builder.Services.AddOidcAuthentication(options =>
                {
                // Replace the Okta placeholders with your Okta values in the appsettings.json file.
                    options.ProviderOptions.Authority = oktaSettings.Authority;
                    options.ProviderOptions.ClientId = oktaSettings.ClientId;
                    options.ProviderOptions.ResponseType = oktaSettings.ResponseType;
                });

                builder.Services.AddApiAuthorization();

            }
            catch (NullReferenceException)
            {
                Console.WriteLine("No Configuration for AzureAdB2C");
            }

            return builder;
        }

        internal static WebAssemblyHostBuilder ConfigureAzureB2C(this WebAssemblyHostBuilder builder, IAppSettings settings)
        {

            try
            {
                const string sectionName = "AzureAdB2C";
                var section = settings.GetSection<object>(sectionName);

                if (section != null)
                {

                    builder.Services.AddMsalAuthentication(options =>
                    {
                        builder.Configuration.Bind(sectionName, options.ProviderOptions.Authentication);

                        options.ProviderOptions.DefaultAccessTokenScopes.Add("openid");

                        options.ProviderOptions.LoginMode = "redirect";
                    });

                    //.AddAccountClaimsPrincipalFactory<ApplicationAuthenticationState, CustomUserAccount, CustomAccountFactory>();

                }
            }
            catch (NullReferenceException)
            {
                Console.WriteLine("No Configuration for AzureAdB2C");
            }

            return builder;
        }

        internal static WebAssemblyHostBuilder ConfigureAppAuthorization(this WebAssemblyHostBuilder builder, IAppSettings settings)
        {
            builder.Services.AddAuthorizationCore(config =>
            {
                config.AddPolicy(ClaimsAuthorizeView.PolicyName, policy =>
                {
                    var cr = new ClaimsRequirement();
                    policy.Requirements.Add(cr);
                });

                config.DefaultPolicy = config.GetPolicy(ClaimsAuthorizeView.PolicyName);
            });

            return builder;
        }


        internal class OktaSettings
        {
            public string Authority { get; set; }
            public string ClientId { get; set; }
            public string ResponseType { get; set; }
        }

    }
}
