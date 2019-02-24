using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Trident.Contracts;
using Trident.Data.Contracts;
using Trident.Domain;
using Trident.Search;
using TestHelpers.TestScopes;

namespace Trident.Tests.Business
{
    [TestClass]
    public class ReadOnlyManagerBaseTests
    {
        [TestMethod]
        public void ReadOnlyManagerBase_()
        {
            Assert.Fail("Needs Tests");
        }
        
        private class DefaultTestScope : TestScope<ITestDeriviedReadOnlyManager>
        {
            private Mock<IReadOnlyProvider<TestEntity>> ProviderMock;
           
            public DefaultTestScope()
            {
                ProviderMock = new Mock<IReadOnlyProvider<TestEntity>>();
                InstanceUnderTest = new TestDerivedReadOnlyManager(ProviderMock.Object);
            }
        }

        private class TestDerivedReadOnlyManager : Trident.Business.ReadOnlyManagerBase<TestEntity>, ITestDeriviedReadOnlyManager
        {
            public TestDerivedReadOnlyManager(IReadOnlyProvider<TestEntity> provider)
                : base(provider)
            {

            }
        }


        private class TestEntity : EntityBase<int> { }

        private interface ITestDeriviedReadOnlyManager : IReadOnlyManager<TestEntity>
        {
        }
    }
}



