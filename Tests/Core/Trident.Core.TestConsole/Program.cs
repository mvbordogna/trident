using Microsoft.Extensions.Configuration;
using System;
using System.Reflection;
using Trident.Core.TestRepositories;
using Trident.Data.Contracts;
using Trident.TestTargetProject;
using Trident.TestTargetProject.Domain;

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

            var testRepo = appContext.IocProvider.Get<IRepository<TestTargetProject.Domain.Organisation>>();

            Organisation temp = null;

            testRepo.Insert(temp = new Organisation()
            {
                Id = Guid.NewGuid(),
                Name = "MyOrg",
                Created = DateTimeOffset.Now
            }).Wait();

            var result = testRepo.GetById(temp.Id).Result;

            var myEntity = result;
        }
    }
}
