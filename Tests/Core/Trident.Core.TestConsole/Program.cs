using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Trident.Contracts;
using Trident.Core.TestRepositories;
using Trident.EFCore;
using Trident.Search;
using Trident.Search.Axioms;
using Trident.TestTargetProject;
using Trident.TestTargetProject.Domain;

namespace Trident.Core.TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var a = 8;






            var appContext = TridentOptionsBuilder.Initialize(new TridentOptions()
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

                AutoDetectConfigFiles = true
            },
            (tBuilder) =>
            {

                //add additional configs here, must be using json config style
                tBuilder.AddEnvironmentVariables();

            });

            var x = AxiomFilterBuilder.CreateFilter()
                .StartGroup()
                    .AddAxiom(new Axiom()
                    {
                        Key = "x",
                        Field = "Name",
                        Operator = CompareOperators.eq,
                        Value = "safasd"

                    })
                    .Or()
                    .StartGroup()            
                        .AddAxiom(new Axiom()
                        {
                            Field = "Name",
                            Operator = CompareOperators.eq,
                            Value = "safasd"

                        }).And().AddAxiom(new Axiom()
                        {
                            Field = "Name",
                            Key = "Name2",
                            Operator = CompareOperators.eq,
                            Value = "safasd"

                        })
                    .EndGroup()
                .EndGroup()
                .Build();


            var testRepo = appContext.ServiceLocator.Get<ISearchRepository<Organisation>>();            

            var criteria = new SearchCriteria();
            // var converter = new GenericEnumValueConverter<OrganisationTypes>(nameof(Organisation.Status)) as Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter;

            //criteria.Filters[nameof(Organisation.Id)] = Guid.Parse("bf682b2a-3e50-4165-b432-c64ed57851ec");

            criteria.Filters[nameof(Organisation.Name)] = new Compare()
            {
                Value = "bla",
                Operator = CompareOperators.contains
            };

            criteria.Filters[nameof(Organisation)] = x;
            criteria.Filters[nameof(Organisation.Age)] = 10;

            var result = testRepo.SearchSync(criteria, new string[] { nameof(Organisation.Status), nameof(Organisation.Departments) });


            var org = result.Results.First();

            org.Departments.Add(new Department()
            {
                Id = Guid.NewGuid(),
                Name = "dept1"
            });

            testRepo.UpdateSync(org);


  
             result = testRepo.SearchSync(criteria, new string[] { nameof(Organisation.Status), nameof(Organisation.Departments) });
            org = result.Results.First();

            //criteria.Filters[nameof(Organisation.Id)] = temp.Id;
            //result = testRepo.Search(criteria, new string[] { nameof(Organisation.Status) }).Result;

            var myEntity = result;
        }
    }
}
