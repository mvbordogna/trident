using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Trident.Data;
using Trident.Data.Contracts;
using Trident.EFCore;
using Trident.Testing.TestScopes;
using Trident.Contracts.Configuration;

namespace Trident.Core.Tests.Data
{

    [TestClass]
    [Ignore]
    public class SharedConnectionStringResolverTests
    {
        [TestMethod]
        [ExpectedException(typeof(ConfigurationErrorsException))]
        public void SharedConnectionStringResolver_GetConnectionString_Returns_Null_For_SharedDataSource_Not_Configured()
        {
            //setup
            var scope = new DefaultTestScope();

            //act
            scope.InstanceUnderTest.GetConnectionString(scope.DBConnectionStringBadKey);
        }

        [TestMethod]
        public void SharedConnectionStringResolver_GetConnectionString_Returns_ConnectionString_When_Found()
        {
            //setup
            var scope = new DefaultTestScope();
            //act
            var actual = scope.InstanceUnderTest.GetConnectionString(scope.DBConnectionStringKey);

            //asert
            Assert.AreEqual(scope.ExpectedConnString, actual);
        }

        [TestMethod]
        public void SharedConnectionStringResolver_GetConnectionString_Returns_ConnectionString_Not_Found_When_Found()
        {
            //setup
            var scope = new DefaultTestScope();
            scope.ConnectionStringSettingsMock.Setup(x => x[scope.DBConnectionStringKey])
                  .Returns(null as ConnectionStringSettings);

            //act
            var actual = scope.InstanceUnderTest.GetConnectionString(scope.DBConnectionStringKey);

            //asert
            Assert.IsNull(actual);
        }


        [TestMethod]
        [ExpectedException(typeof(ConfigurationErrorsException))]
        public void SharedConnectionStringResolver_GetConnection_Returns_Null_For_SharedDataSource_Not_Configured()
        {
            //setup
            var scope = new DefaultTestScope();

            //act
            using (scope.InstanceUnderTest.GetConnection(scope.DBConnectionStringBadKey)) { }
        }

        [TestMethod]
        [Ignore]
        public void SharedConnectionStringResolver_GetConnection_Returns_ConnectionString_When_Found_Defaults_To_SQL_Provider()
        {
            //setup
            var scope = new DefaultTestScope();
            scope.TestConnStringSetting.ProviderName = "";
            //act
            using (var actual = scope.InstanceUnderTest.GetConnection(scope.DBConnectionStringKey))
            {
                //asert
                Assert.AreEqual(scope.ExpectedConnString, actual.ConnectionString);
            }
        }


        [TestMethod]
        [Ignore]
        public void SharedConnectionStringResolver_GetConnection_Returns_ConnectionString_When_Found_()
        {
            //setup
            var scope = new DefaultTestScope();
            //act
            using (var actual = scope.InstanceUnderTest.GetConnection(scope.DBConnectionStringKey))
            {
                //asert
                Assert.AreEqual(scope.ExpectedConnString, actual.ConnectionString);
            }
        }

        [TestMethod]
        public void SharedConnectionStringResolver_GetConnection_Returns_ConnectionString_Not_Found_When_Found()
        {
            //setup
            var scope = new DefaultTestScope();
            scope.ConnectionStringSettingsMock.Setup(x => x[scope.DBConnectionStringKey])
                  .Returns(null as ConnectionStringSettings);

            //act
            using (var actual = scope.InstanceUnderTest.GetConnection(scope.DBConnectionStringKey))
            {
                //asert
                Assert.IsNull(actual);
            }
        }


        private class DefaultTestScope : TestScope<ISharedConnectionStringResolver>
        {
            public string ExpectedConnString = "Data Source=.; Initial Catalog=TestDB; Integrated Security=true; MultipleActiveResultSets=false;";
            public string DBConnectionStringKey = "DefaultDB";
            public string DBConnectionStringBadKey = "BadDBName";

            public ConnectionStringSettings TestConnStringSetting { get; }


            public Mock<IConnectionStringSettings> ConnectionStringSettingsMock { get; }

            public DefaultTestScope()
            {
                TestConnStringSetting = new ConnectionStringSettings(this.DBConnectionStringKey, this.ExpectedConnString);
                TestConnStringSetting.ProviderName = "System.Data.SqlClient";
                ConnectionStringSettingsMock = new Mock<IConnectionStringSettings>();

                ConnectionStringSettingsMock.Setup(x => x[DBConnectionStringKey])
                    .Returns(TestConnStringSetting);

                InstanceUnderTest = new SharedConnectionStringResolver(ConnectionStringSettingsMock.Object, new DBProviderAbstractFactory());
            }
        }

    }
}
