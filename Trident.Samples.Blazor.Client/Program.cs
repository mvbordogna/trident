using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Trident.Samples.Blazor.Client;
using Blazored.LocalStorage;
using Blazored.SessionStorage;
using Trident.UI.Blazor.Logging;
using Trident.UI.Blazor.Logging.AppInsights;
using Trident.Samples.Blazor.Client.Configuration;
using Microsoft.Extensions.Logging;
using Trident.IoC;
using Autofac;
using Trident.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Trident.Sample.UI;
using Trident.Samples.Contracts.Services;
using Trident.Sample.UI.Servcies;

public class Program
{
    static async Task Main(params string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);


        builder.RootComponents.Add<App>("#app");
        builder.RootComponents.Add<HeadOutlet>("head::after");
        builder.ConfigureSecurity();
        builder.ConfigureHttpClients();
        builder.ConfigureContainer(new IoCServiceProviderFactory<AutofacIoCProvider>(
            (provider) => {                                 

                provider.Populate(builder.Services);                
                provider.RegisterModules(new Type[] {                       
                        typeof(Trident.UI.Blazor.PackageModule),
                        typeof(Trident.Sample.UI.PackageModule),                   
                        typeof(Trident.Samples.Blazor.Client.PackageModule)
                    })
                .RegisterSelf();

            }));


        var appSettings = new AppSettings(builder.Configuration);

        //builder.Logging.SetMinimumLevel(LogLevel.Debug);
        //builder.Logging.AddProvider(new DefaultLogProvider());


        // builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
        //builder.Services.AddBlazoredSessionStorage();
        //builder.Services.AddBlazoredLocalStorage();


        builder.Logging.AddBlazorClientLogging(
            new LoggingConfiguration()
            {
                LogLevel = LogLevel.Trace
            }
            ,
            new AppInsightsConfig()
            {
                ConnectionString = "",
                InstrumentationKey = "",
                DisableFetchTracking = false,
                EnableCorsCorrelation = true,
                EnableRequestHeaderTracking = true,
                EnableResponseHeaderTracking = true
            }
        );

        var host = builder.Build();
        await host.RunAsync();
    }

}


