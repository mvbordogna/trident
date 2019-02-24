using System.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Trident.Common;
using Trident.Contracts.Enums;
using Trident.Data;
using Trident.Data.Contracts;
using TestHelpers.TestScopes;

namespace Trident.Tests.Data
{
    [TestClass]
    public class SharedConnectionStringResolverTests
    {
        [TestMethod]
        [ExpectedException(typeof(ConfigurationErrorsException))]
        public void SharedConnectionStringResolver_GetConnectionString_Returns_Null_For_SharedDataSource_Not_Configured()
        {
            //setup
            var scope = new DefaultTestScope();

            //act
            scope.InstanceUnderTest.GetConnectionString(SharedDataSource.Undefined);
        }

        [TestMethod]
        public void SharedConnectionStringResolver_GetConnectionString_Returns_ConnectionString_When_Found()
        {
            //setup
            var scope = new DefaultTestScope();
            //act
            var actual = scope.InstanceUnderTest.GetConnectionString(SharedDataSource.DefaultDB);

            //asert
            Assert.AreEqual(scope.ExpectedConnString, actual);
        }

        [TestMethod]
        public void SharedConnectionStringResolver_GetConnectionString_Returns_ConnectionString_Not_Found_When_Found()
        {
            //setup
            var scope = new DefaultTestScope();
            scope.ConnectionStringSettingsMock.Setup(x => x[scope.DBConnectionStringKey])
                  .Returns(null as System.Configuration.ConnectionStringSettings);

            //act
            var actual = scope.InstanceUnderTest.GetConnectionString(SharedDataSource.DefaultDB);

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
            using (scope.InstanceUnderTest.GetConnection(SharedDataSource.Undefined)) { }
        }

        [TestMethod]
        public void SharedConnectionStringResolver_GetConnection_Returns_ConnectionString_When_Found_Defaults_To_SQL_Provider()
        {
            //setup
            var scope = new DefaultTestScope();
            scope.TestConnStringSetting.ProviderName = "";
            //act
            using (var actual = scope.InstanceUnderTest.GetConnection(SharedDataSource.DefaultDB))
            {
                //asert
                Assert.AreEqual(scope.ExpectedConnString, actual.ConnectionString);
            }
        }


        [TestMethod]
        public void SharedConnectionStringResolver_GetConnection_Returns_ConnectionString_When_Found_()
        {
            //setup
            var scope = new DefaultTestScope();
            //act
            using (var actual = scope.InstanceUnderTest.GetConnection(SharedDataSource.DefaultDB))
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
                  .Returns(null as System.Configuration.ConnectionStringSettings);

            //act
            using (var actual = scope.InstanceUnderTest.GetConnection(SharedDataSource.DefaultDB))
            {
                //asert
                Assert.IsNull(actual);
            }
        }


        private class DefaultTestScope : TestScope<ISharedConnectionStringResolver>
        {
            public string ExpectedConnString = "Data Source=.; Initial Catalog=TestDB; Integrated Security=true; MultipleActiveResultSets=false;";
            public string DBConnectionStringKey = nameof(SharedDataSource.DefaultDB);

            public System.Configuration.ConnectionStringSettings TestConnStringSetting { get; }


            public Mock<IConnectionStringSettings> ConnectionStringSettingsMock { get; }

            public DefaultTestScope()
            {
                TestConnStringSetting = new System.Configuration.ConnectionStringSettings(this.DBConnectionStringKey, this.ExpectedConnString);
                TestConnStringSetting.ProviderName = "System.Data.SqlClient";
                ConnectionStringSettingsMock = new Mock<IConnectionStringSettings>();

                ConnectionStringSettingsMock.Setup(x => x[DBConnectionStringKey])
                    .Returns(TestConnStringSetting);

                InstanceUnderTest = new SharedConnectionStringResolver(ConnectionStringSettingsMock.Object);
            }
        }

    }
}
