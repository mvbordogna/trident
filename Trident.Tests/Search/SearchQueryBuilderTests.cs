using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Trident.Search;
using TestHelpers.TestScopes;

namespace Trident.Tests.Search
{
    [TestClass]
    public class SearchQueryBuilderTests
    {

        [TestMethod]
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
