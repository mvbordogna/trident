//using System;
//using System.Collections.Generic;
//using System.Text;
//using Trident.Business;
//using Trident.Contracts;
//using Trident.Domain;
//using Trident.Data;
//using Trident.Search;
//using System.Threading.Tasks;
//using System.Linq.Expressions;
//using System.Linq;
//using Trident.EFCore;
//using Trident.Data.Contracts;
//using Trident.Workflow;
//using Trident.Validation;
//using Autofac.Core;
//using Autofac;

//namespace Trident.Core.Tests
//{

//    public class Test
//    {
//        public void Tests()
//        {
//            AxiomFilterBuilder.CreateFilter()
//                .AddAxiom(new Axiom(new Compare() { Operator = CompareOperators.eq }))
//        }
//    }


//    public class MyDataClass : EntityBase<Guid>
//    {
//        public string MyName
//        {
//            get;
//            set;
//        }
//    }

//    public class MyDataClassDto
//    {
//        public string MyName
//        {
//            get;
//            set;
//        }
//    }



//    public interface IMyDataClassManager : IManager<Guid, MyDataClass>
//    {

//    }

//    public class MyDataClassManager : ManagerBase<Guid, MyDataClass>, IMyDataClassManager
//    {
//        public MyDataClassManager(IMyDataClassProvider provider) : base(provider, )
//        {

//        }
//    }


//    public interface IMyDataClassProvider : IProvider<Guid, MyDataClass> { };

//    public class MyDataClassProvider : ProviderBase<Guid, MyDataClass>, IMyDataClassProvider
//    {
//        public MyDataClassProvider(ISearchRepository<MyDataClass> repository) : base(repository)
//        {

//        }
//    }


//    public interface IMyDataClassRepository : ISearchRepository<MyDataClass> { }

//    public class MyDataClassRepository : EFCoreSearchRepositoryBase<MyDataClass>, IMyDataClassRepository
//    {
//        public MyDataClassRepository(ISearchResultsBuilder resultsBuilder, ISearchQueryBuilder queryBuilder, IAbstractContextFactory abstractContextFactory)
//            : base(resultsBuilder, queryBuilder, abstractContextFactory)
//        {
//        }
//    }

//    public class Class1
//    {

//        public void TestMe()
//        {
//            var man = new MyDataClassManager(null);
//            //man.Get(x=> x.MyName == )

//            man..PatchSync(Guid.NewGuid(), false, new Action<MyDataClass>[]
//            {
//                      x=> x.MyName = "tommy",

//            });


//            "(x,.myprop != \"bob\"  and x.byg == 10"
//        }
//    }




//    public class AssureNameCaonotChangevalidationRule : Validation.ValidationRuleBase<BusinessContext<MyDataClass>>
//    {
//        public override int RunOrder => 1;

//        public override Task Run(BusinessContext<MyDataClass> context, List<ValidationResult> errors)
//        {
//            if (context.Original.MyName != context.Target.MyName)
//            {
//                errors.Add(new ValidationResult(nameof(context.Target.MyName)));
//                throw new ValidationRollupException(new ValidationResult(nameof(context.Target.MyName));
//            }

//            return Task.CompletedTask;
//        }
//    }

//    public enum codes
//    {

//    }

//    public class XAssureNameCaonotChangevalidationRule : Validation.PropertyExpressionValidationRule<BusinessContext<MyDataClass>, MyDataClass, codes>
//    {
//        public override int RunOrder => throw new NotImplementedException();

//        protected override void ConfigureRules()
//        {
//            this.AddRule(nameof(MyDataClass.MyName), x => x.MyName != null);


//        }
//    }


//    public class MyWorkflowTask : Workflow.WorkflowTaskBase<BusinessContext<MyDataClass>>
//    {

//        public MyWorkflowTask(IEmailProver provider)
//        {

//        }

//        public override int RunOrder => 100;

//        public override OperationStage Stage => OperationStage.All;

//        public override Task<bool> Run(BusinessContext<MyDataClass> context)
//        {
//            if (context.Original.MyName != context.Target.MyName)
//            {
//                //send email provider
//            }

//            return true;
//        }

//        public override async Task<bool> ShouldRun(BusinessContext<MyDataClass> context)
//        {
//            return context.Original.MyName.Length > 2;
//        }
//    }



//}
