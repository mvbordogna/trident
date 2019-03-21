using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Trident.Business;
using Trident.Data.Contracts;
using Trident.Domain;
using Trident.Search;
using Trident.Testing.TestScopes;

namespace Trident.Tests.Business
{


    [TestClass]
    public class ReadOnlyProviderBaseTests
    {
        [TestMethod]
        [Ignore]
        public void ReadOnlyProviderBase_()
        {
           Assert.Fail("Needs Tests");
        }

        [TestMethod]
        [Ignore]
        public void ReadOnlyProviderBase_GetByIds()
        {
            var scope = new DefaultTestScope();
            
            Assert.Fail("Needs Tests");
        }


        private class DefaultTestScope : TestScope<ITestDerivedReadOnlyProvider>
        {
            private Mock<ISearchRepository<TestEntity>> RepositoryMock;

            public DefaultTestScope()
            {
                RepositoryMock = new Mock<ISearchRepository<TestEntity>>();
                InstanceUnderTest = new ReadOnlyTestDerivedProvider(RepositoryMock.Object);
            }
        }


        private class ReadOnlyTestDerivedProvider : ProviderBase<int, TestEntity>, ITestDerivedReadOnlyProvider
        {
            public ReadOnlyTestDerivedProvider(ISearchRepository<TestEntity> repository)
                : base(repository) { }
        }

        private class TestEntity : EntityBase<int> { }
        private interface ITestDerivedReadOnlyProvider : IReadOnlyProvider<TestEntity> { }
    }

}
