using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Trident.Testing.TestScopes;
using Trident.IoC;
using Trident.Data.EntityFramework.Contracts;
using Trident.Data.Contracts;
using Trident.Contracts.Enums;
using System.Linq;
using System.Data;
using Moq;
using Trident.EF6;
using Trident.EF6.Contracts;

namespace Trident.Framework.EF6.Tests
{
    [TestClass]
    public class EFSharedContextFactoryTests
    {
        [TestMethod]
        public void EFSharedContextFactory_Invokes_ServiceLocator_With_Expected_Args()
        {
            var scope = new DefaultScope();
            var expected = scope.EFDbContextMock.Object;
            var actual = scope.InstanceUnderTest.Get(scope.TestEntityType, scope.TestDataSource);
            Assert.IsNotNull(actual);
            Assert.AreEqual(expected, actual);
        }

        private class DefaultScope : TestScope<EFSharedContextFactory>
        {
            Mock<IIoCServiceLocator> ServiceLocatorMock { get; }
            Mock<ISharedConnectionStringResolver> ConnectionStringResolverMock { get; }
            Mock<IModelBuilderFactory> ModalBuilderFactoryMock { get; }
            public Mock<IEFDbContext> EFDbContextMock { get; }
            public SharedDataSource TestDataSource { get; set; }
            public Type TestEntityType { get; set; }
            public string TestCompanyCode { get; set; }
            public string TestConnectionString { get; }
            public Mock<IDbModelBuilder> DbModelBuilderMock { get; private set; }

            public Mock<IDbConnection> DbConnectionMock { get; }

            public DefaultScope()
            {
                TestEntityType = typeof(TestEntity);
                TestDataSource = SharedDataSource.DefaultDB;
                TestConnectionString = "TestConnStr";
                DbConnectionMock = new Mock<IDbConnection>();

                ServiceLocatorMock = new Mock<IIoCServiceLocator>();
                ConnectionStringResolverMock = new Mock<ISharedConnectionStringResolver>();
                ModalBuilderFactoryMock = new Mock<IModelBuilderFactory>();
                EFDbContextMock = new Mock<IEFDbContext>();
                DbModelBuilderMock = new Mock<IDbModelBuilder>();

                ConnectionStringResolverMock.Setup(x=> x.GetConnection(TestDataSource)).Returns(DbConnectionMock.Object);
                ModalBuilderFactoryMock.Setup(x=> x.Get(TestDataSource.ToString())).Returns(DbModelBuilderMock.Object);


                ServiceLocatorMock.Setup(z=> z.GetNamed<IEFDbContext>(It.IsAny<string>(), It.Is<Parameter[]>(x =>
                     x.First(y => y.Value as IDbConnection == DbConnectionMock.Object) != null &&
                     x.First(y => y.Value as IDbModelBuilder == DbModelBuilderMock.Object) != null)
                     ))
                    .Returns(EFDbContextMock.Object);

                InstanceUnderTest = new EFSharedContextFactory(ConnectionStringResolverMock.Object, ModalBuilderFactoryMock.Object, ServiceLocatorMock.Object);
            }
        }

        private class TestEntity
        {

        }




    }
}

