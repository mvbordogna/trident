using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Trident.Search;
using Trident.Testing.TestScopes;

namespace Trident.Tests.Search
{
    [TestClass]
    public class SearchQueryBuilderTests
    {

        [TestMethod]
        [Ignore]
        public void SearchQueryBuilder_()
        {
            Assert.Fail("Needs tests");
        }

        private class DefaultTestScope : TestScope<ISearchQueryBuilder>
        {
            public DefaultTestScope()
            {
                MockComplexFilterFactory = new Mock<IComplexFilterFactory>();
                InstanceUnderTest = new SearchQueryBuilder(MockComplexFilterFactory.Object);
            }

            public Mock<IComplexFilterFactory> MockComplexFilterFactory { get; }
        }

    }
}
