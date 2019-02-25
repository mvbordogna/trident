using Microsoft.VisualStudio.TestTools.UnitTesting;
using Trident.Search;
using Trident.Testing.TestScopes;

namespace Trident.Tests.Search
{

    [TestClass]
    public class SearchResultsBuilderTests
    {

        [TestMethod]
        [Ignore]
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
