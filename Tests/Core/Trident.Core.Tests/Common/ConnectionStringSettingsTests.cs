using Microsoft.VisualStudio.TestTools.UnitTesting;
using Trident.Common;
using Trident.Contracts.Configuration;
using Trident.Testing.TestScopes;

namespace Trident.Core.Tests.Common
{
    [TestClass]
    public class ConnectionStringSettingsTests
    {
        [TestMethod]
        [Ignore]
        public void ConnectionStringSettings_Retrieves_Value_Using_String_Key()
        {
            //setup
            var scope = new DefaultTestScope();

            //act
            var actual = scope.InstanceUnderTest[scope.TestKey];

            //assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(scope.TestConnString, actual.ConnectionString);

        }

        [TestMethod]
        public void ConnectionStringSettings_Returns_Null_When_Key_Not_Found()
        {
            //setup
            var scope = new DefaultTestScope();

            //act
            var actual = scope.InstanceUnderTest["BlaBla"];

            //assert
            Assert.IsNull(actual);
        }

        [TestMethod]
        [Ignore]
        public void ConnectionStringSettings_Retrieves_Value_Using_Int_Index()
        {
            //setup
            var scope = new DefaultTestScope();

            //act
            var actual = scope.InstanceUnderTest[scope.TestIndex];

            //assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(scope.TestConnString, actual.ConnectionString);

        }

        [TestMethod]
        public void ConnectionStringSettings_Returns_Null_When_Index_Not_Found()
        {
            //setup
            var scope = new DefaultTestScope();

            //act
            var actual = scope.InstanceUnderTest[100000000];

            //assert
            Assert.IsNull(actual);
        }

        private class DefaultTestScope : TestScope<IConnectionStringSettings>
        {
            public int TestIndex { get; } = 1;
            public string TestKey { get; } = "IntegrationTestDB";
            public string TestConnString { get; } = "Data Source=.; Initial Catalog=TestDB; Integrated Security=true; MultipleActiveResultSets=false;";

            public DefaultTestScope()
            {
                InstanceUnderTest = new XmlConnectionStringSettings();
            }
        }

    }
}
