using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Trident.Business;
using Trident.Contracts;
using Trident.Data.Contracts;
using Trident.Domain;
using Trident.Mapper;
using Trident.Transactions;
using Trident.Validation;
using Trident.Workflow;
using System.Threading.Tasks;
using TestHelpers.TestScopes;

namespace Trident.Tests.Business
{
    [TestClass]
    public class ManagerBaseTests
    {

        [TestMethod]
        public async Task ManagerBase_()
        {

            Assert.Fail("Needs Tests");

        }

        private class DefaultTestScope : TestScope<ITestDerivedManager>
        {
            public Mock<IProvider<int, TestEntity>> ProviderMock { get; }

            public Mock<IMapperRegistry> MapperMock { get; }              
            public Mock<IValidationManager> ValidationManager { get; }
            public Mock<IWorkflowManager> WorkflowManager { get; }


            public DefaultTestScope()
            {
                ProviderMock = new Mock<IProvider<int, TestEntity>>();

                MapperMock = new Mock<IMapperRegistry>();              
                ValidationManager = new Mock<IValidationManager>();
                WorkflowManager = new Mock<IWorkflowManager>();



                InstanceUnderTest = new TestDerivedManager(
                    MapperMock.Object,
                    ProviderMock.Object,                   
                    ValidationManager.Object,
                    WorkflowManager.Object
                    );


                  
            }
        }

        private class TestDerivedManager : ManagerBase<int, TestEntity>, ITestDerivedManager
        {
            public TestDerivedManager(
                IMapperRegistry mapper, 
                IProvider<int, TestEntity> provider,               
                IValidationManager validationManager,
                IWorkflowManager workflowManager = null)
                : base(mapper, provider, validationManager, workflowManager)
            { }
        }


        private class TestEntity : EntityBase<int>
        {

        }

        private interface ITestDerivedManager : IManager<int, TestEntity>
        {
        }
    }


}



