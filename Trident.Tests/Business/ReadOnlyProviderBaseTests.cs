using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Trident.Domain;
using Trident.Search;
using Trident.Testing.TestScopes;

namespace Trident.Tests.Business
{


    [TestClass]
    public class ReadOnlyProviderBaseTests
    {
        [TestMethod]
        public void ReadOnlyProviderBase_()
        {
            Assert.Fail("Needs Tests");
        }

        [TestMethod]
        public void ReadOnlyProviderBase_GetByIds()
        {
            var scope = new DefaultTestScope();
            
            Assert.Fail("Needs Tests");
        }


        private class DefaultTestScope : TestScope<ITestDerivedReadOnlyProvider>
        {
            private Mock<ISearchRepository<TestEntity, TestEntity, SearchCriteria>> RepositoryMock;

            public DefaultTestScope()
            {
                RepositoryMock = new Mock<ISearchRepository<TestEntity, TestEntity, SearchCriteria>>();
                InstanceUnderTest = new ReadOnlyTestDerivedProvider(RepositoryMock.Object);
            }
        }


        private class ReadOnlyTestDerivedProvider : Trident.Business.ProviderBase<int, TestEntity>, ITestDerivedReadOnlyProvider
        {
            public ReadOnlyTestDerivedProvider(ISearchRepository<TestEntity, TestEntity, SearchCriteria> repository)
                : base(repository) { }
        }

        private class TestEntity : EntityBase<int> { }
        private interface ITestDerivedReadOnlyProvider : Trident.Data.Contracts.IReadOnlyProvider<TestEntity> { }
    }

}
