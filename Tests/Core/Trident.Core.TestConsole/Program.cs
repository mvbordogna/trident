using Microsoft.Extensions.Configuration;
using System;
using System.Reflection;
using Trident.Core.TestRepositories;
using Trident.Data.Contracts;
using Trident.TestTargetProject;

namespace Trident.Core.TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var a = 8;

            var appContext = Trident.Initialize(new TridentConfigurationOptions()
            {
                TargetAssemblies = new Assembly[]
                  {
                     typeof(TestManager).Assembly,
                     typeof(TestRepository).Assembly,
                     typeof(Program).Assembly
                    
                  },
                EnableTransactions = true,
                ModuleTypes = new Type[] {
                     typeof(AppModule),
                     typeof(BizModule)
               },

               AutoDetectConfigFiles = true,           
            }, (builder)=> {
                //add additional configs here, must be using json config style
                builder.AddEnvironmentVariables();
            });

           appContext.IocProvider.VerifyAndThrow();

            var testRepo = appContext.IocProvider.Get<IRepository<TestTargetProject.Domain.TestEntity>>();
            var result = testRepo.GetById(Guid.Parse("e9ac7c9a-961c-4d53-a255-7372e6f223db")).Result;

            var myEntity = result;
        }
    }
}
