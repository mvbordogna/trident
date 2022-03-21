using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Reflection;
using Trident.Contracts.Enums;
using Trident.Data;
using Trident.Data.Contracts;
using Trident.IoC;
using Trident.Testing.TestScopes;

namespace Trident.Tests.Data
{
    [TestClass]
    public class AbstractContextFactoryTests
    {
        [TestMethod]
        public void AbstractContextFactory_Create_Returns_Shared_Context()
        {
            using (var scope = new DefaultScope())
            {
                var actual = scope.InstanceUnderTest.Create<IContext>(typeof(SharedSourceTestEntity));
                Assert.IsNotNull(actual);
                Assert.IsInstanceOfType(actual, typeof(ITestSharedContext));
            }
        }

        [TestMethod]
        public void AbstractContextFactory_Create_Returns_Default_Shared_Context_No_Attribute()
        {
            using (var scope = new DefaultScope())
            {
                var actual = scope.InstanceUnderTest.Create<IContext>(typeof(DefaultedSharedSourceTestEntity));
                Assert.IsNotNull(actual);
                Assert.IsInstanceOfType(actual, typeof(ITestSharedContext));
            }
        }

        [TestMethod]
        public void AbstractContextFactory_Create_Returns_Default_Shared_Context_Attribute_With_Undefined()
        {
            using (var scope = new DefaultScope())
            {
                var actual = scope.InstanceUnderTest.Create<IContext>(typeof(UndefinedSharedSourceTestEntity));
                Assert.IsNotNull(actual);
                Assert.IsInstanceOfType(actual, typeof(ITestSharedContext));
            }
        }

        [TestMethod]
        public void AbstractContextFactory_Skips_Clear_Dictionary_if_Null()
        {
            try
            {
                using (var scope = new DefaultScope())
                {
                    var flags = BindingFlags.Instance | BindingFlags.NonPublic;

                    var contextCache = typeof(AbstractContextFactory).GetField("_contextCache", flags);

                    contextCache.SetValue(scope.InstanceUnderTest, null);
                }
            }
            catch (NullReferenceException)
            {
                Assert.Fail("Clear was called on the context dictionary and it was not null reference protected.");
            }
        }


        private class DefaultScope : DisposableTestScope<AbstractContextFactory>
        {
            public DefaultScope()
            {
                ServiceLocatorMock = new Mock<IIoCServiceLocator>();
                SharedContextFactoryMock = new Mock<ISharedContextFactory<IContext>>();
                SharedContextMock = new Mock<ITestSharedContext>();


                ServiceLocatorMock.Setup(x => x.GetNamed<ISharedContextFactory>(SharedDataSource.DefaultDB.ToString()))
                    .Returns(SharedContextFactoryMock.Object);

                SharedContextFactoryMock.Setup(x => x.Get(typeof(SharedSourceTestEntity), SharedDataSource.DefaultDB.ToString())).
                    Returns(SharedContextMock.Object);

                SharedContextFactoryMock.Setup(x => x.Get(typeof(UndefinedSharedSourceTestEntity), SharedDataSource.DefaultDB.ToString())).
                 Returns(SharedContextMock.Object);

                SharedContextFactoryMock.Setup(x => x.Get(typeof(DefaultedSharedSourceTestEntity), SharedDataSource.DefaultDB.ToString())).
                 Returns(SharedContextMock.Object);

                InstanceUnderTest = new AbstractContextFactory(ServiceLocatorMock.Object);
            }

            public Mock<ISharedContextFactory<IContext>> SharedContextFactoryMock { get; private set; }
            public Mock<IIoCServiceLocator> ServiceLocatorMock { get; private set; }
            public Mock<ITestSharedContext> SharedContextMock { get; private set; }


        }

        public interface ITestSharedContext : IContext { }

        public class UnResolvableDataSourceEntity { };

        [UseSharedDataSource(nameof(SharedDataSource.DefaultDB))]
        public class SharedSourceTestEntity
        {
            public Guid Id { get; set; }
        }

        [UseSharedDataSource()]
        public class UndefinedSharedSourceTestEntity
        {
            public Guid Id { get; set; }
        }

        public class DefaultedSharedSourceTestEntity
        {
            public Guid Id { get; set; }
        }
    }
}
