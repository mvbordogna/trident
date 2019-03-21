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
    public class ProviderBaseTests
    {
        [TestMethod]
        [Ignore]
        public void ProviderBase_()
        {
            Assert.Fail("Needs Tests");
        }

        private class DefaultTestScope : TestScope<ITestDerivedProvider>
        {
            private Mock<ISearchRepository<TestEntity>> RepositoryMock;

            public DefaultTestScope()
            {
                RepositoryMock = new Mock<ISearchRepository<TestEntity>>();
                InstanceUnderTest = new TestDerivedProvider(RepositoryMock.Object);
            }
        }

        private class TestDerivedProvider : ProviderBase<int, TestEntity>, ITestDerivedProvider
        {
            public TestDerivedProvider(ISearchRepository<TestEntity> repository) 
                : base(repository) { }
        }

        private class TestEntity : EntityBase<int> { }

        private interface ITestDerivedProvider : IProvider<int, TestEntity> { }
    }


}
