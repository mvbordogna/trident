using Microsoft.Extensions.Hosting;
using System;
using System.Threading.Tasks;
using Trident.IoC;

namespace Trident.Samples.AzureFunctions
{
    public class Program
    {
        public static async Task Main()
        {
            var hostBuilder = new HostBuilder();

            var host = hostBuilder.UseServiceProviderFactory(new IoCServiceProviderFactory<AutofacIoCProvider>((provider) =>
            {
                provider.Register<TestInject, ITestInject>();
                provider.RegisterModules(new Type[] {
                        typeof(PackageModule)
                    })
                .RegisterSelf();
            })).ConfigureContainer<IIoCProvider>((builder) =>
            {

                return;
            })
               .ConfigureFunctionsWorkerDefaults(builder =>
               {
                   //builder.UseMiddleware<ExceptionLoggingMiddleware>();
                   //builder.UseMiddleware<FunctionsSecurityMiddleware>();
               }).Build();

            await host.RunAsync();
        }
    }
}