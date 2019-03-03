using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Trident.Business;
using Trident.Contracts;
using Trident.Data.Contracts;
using Trident.Domain;
using Trident.Mapper;
using Trident.Validation;
using Trident.Workflow;
using System.Threading.Tasks;
using Trident.Testing.TestScopes;

namespace Trident.Tests.Business
{
    [TestClass]
    public class ManagerBaseTests
    {

        [TestMethod]
        [Ignore]
        public async Task ManagerBase_()
        {

            Assert.Fail("Needs Tests");

        }

        private class DefaultTestScope : TestScope<ITestDerivedManager>
        {
            public Mock<IProvider<int, TestEntity>> ProviderMock { get; }

            public Mock<IMapperRegistry> MapperMock { get; }
            public Mock<IValidationManager<TestEntity>> ValidationManager { get; }
            public Mock<IWorkflowManager<TestEntity>> WorkflowManager { get; }


            public DefaultTestScope()
            {
                ProviderMock = new Mock<IProvider<int, TestEntity>>();

                MapperMock = new Mock<IMapperRegistry>();
                ValidationManager = new Mock<IValidationManager<TestEntity>>();
                WorkflowManager = new Mock<IWorkflowManager<TestEntity>>();

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
                IValidationManager<TestEntity> validationManager,
                IWorkflowManager<TestEntity> workflowManager = null)
                : base(provider, validationManager, workflowManager)
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



