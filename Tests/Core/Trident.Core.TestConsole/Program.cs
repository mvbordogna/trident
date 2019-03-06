using Microsoft.Extensions.Configuration;
using System;
using System.Reflection;
using Trident.Contracts;
using Trident.Core.TestRepositories;
using Trident.EFCore;
using Trident.Search;
using Trident.TestTargetProject;
using Trident.TestTargetProject.Domain;

namespace Trident.Core.TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var a = 8;

            var appContext = Trident.Initialize(new TridentOptions()
            {
                TargetAssemblies = new Assembly[]
                  {
                     typeof(OrganisationManager).Assembly,
                     typeof(TestRepository).Assembly,
                     typeof(Program).Assembly

                  },
                ValidateInitialization = true,
                EnableTransactions = true,
                ModuleTypes = new Type[] {
                     typeof(AppModule),
                     typeof(BizModule)
               },

                AutoDetectConfigFiles = true,
            }, (builder) =>
            {
                //add additional configs here, must be using json config style
                builder.AddEnvironmentVariables();
            });



            var testRepo = appContext.ServiceLocator.Get<ISearchRepository<Organisation>>();

            var testManager = appContext.ServiceLocator.Get<IManager<Guid, Organisation>>();

            //Organisation temp = null;

            //testRepo.InsertSync(temp = new Organisation()
            //{
            //    Id = Guid.NewGuid(),
            //    Name = "MyOrg",
            //    Created = DateTimeOffset.Now
            //    ,               
            //    OrgType = OrganisationTypes.Corp,
                
            //    Status = new OrgStatus()
            //    {
            //        Id = Guid.NewGuid(),
            //        OrgType = OrganisationTypes.LLC
            //    }
            //});
            

            var criteria = new SearchCriteria();
           // var converter = new GenericEnumValueConverter<OrganisationTypes>(nameof(Organisation.Status)) as Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter;

            criteria.Filters[nameof(Organisation.Id)] = Guid.Parse("bf682b2a-3e50-4165-b432-c64ed57851ec");
            var result = testRepo.Search(criteria, new string[] { nameof(Organisation.Status) }).Result;

            //criteria.Filters[nameof(Organisation.Id)] = temp.Id;
            //result = testRepo.Search(criteria, new string[] { nameof(Organisation.Status) }).Result;

            var myEntity = result;
        }
    }
}
