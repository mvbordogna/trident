using Microsoft.VisualStudio.TestTools.UnitTesting;
using Trident.Search;
using TestHelpers.TestScopes;

namespace Trident.Tests.Search
{

    [TestClass]
    public class SearchResultsBuilderTests
    {

        [TestMethod]
        public void SearchResultsBuilder_()
        {
            Assert.Fail("Needs tests");
        }



        private class DefaultTestScope : TestScope<ISearchResultsBuilder>
        {
            public DefaultTestScope()
            {
                InstanceUnderTest = new SearchResultsBuilder();
            }
        }
    }
}
