using System;
using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Trident.Testing.TestScopes;
using Trident.Data.Contracts;
using System.Threading.Tasks;
using Trident.Testing;
using System.Collections.Generic;
using System.Linq;
using Trident.Testing.EF;
using Moq;
using Trident.Domain;
using Trident.EF6;

namespace Trident.Framework.EF6.Tests
{
    [TestClass]
    public class EFRepositoryTests
    {
        [TestMethod]
        [TestCategory(TestCategory.Unit)]
        public async Task Get_Applies_Filter()
        {
            // Arrange
            var scope = new DefaultScope();
            Expression<Func<MockEntity, bool>> filter = x => x.Id == scope.Entity1.Id;
            // Act

            var results = await scope.InstanceUnderTest.Get(filter, null, null);


            // Assert
            Assert.AreEqual(1, results.Count());
            Assert.AreEqual(scope.Entity1.Id, results.FirstOrDefault().Id);
        }

        [TestMethod]
        [TestCategory(TestCategory.Unit)]
        public async Task Get_Does_Not_Apply_Filter()
        {
            // Arrange
            var scope = new DefaultScope();

            // Act

            var results = await scope.InstanceUnderTest.Get(null, null, null);


            // Assert
            Assert.AreEqual(2, results.Count());
        }

       

        private class DefaultScope : TestScope<EFRepositoryBaseConcrete>
        {
            public Mock<IContext> ContextMock { get; }

            public DefaultScope()
            {
                ContextMock = new Mock<IContext>();
                var contextFactory = new Mock<IAbstractContextFactory>();
                contextFactory.Setup(x=> x.Create<IContext>(It.IsAny<Type>())).Returns(ContextMock.Object);
                Entity1 = new MockEntity { Id = Guid.NewGuid(), Leaf = new MockEntity2(), Test = "a", Test2 = "b" };
                Entity2 = new MockEntity { Id = Guid.NewGuid(), Leaf = new MockEntity2(), Test = "c", Test2 = "d" };
                var entities = new List<MockEntity>() { Entity1, Entity2 };
                Query = new Mock<TestDbAsyncEnumerable<MockEntity>>(entities.AsQueryable());               
                ContextMock.Setup(x=> x.Query<MockEntity>(false)).Returns(Query.Object);
                InstanceUnderTest = new EFRepositoryBaseConcrete(contextFactory.Object);
            }

            public MockEntity Entity1 { get; }
            public MockEntity Entity2 { get; }
            public Mock<TestDbAsyncEnumerable<MockEntity>> Query { get; }

        }

        private class EFRepositoryBaseConcrete : EFRepository<MockEntity>
        {
            public EFRepositoryBaseConcrete(IAbstractContextFactory contextFactory) : base(contextFactory) { }
        }
        public class MockEntity:EntityBase<Guid>
        {
            public virtual MockEntity2 Leaf { get; set; }

            public virtual string Test { get; set; }

            public virtual string Test2 { get; set; }

        }

        public class MockEntity2: EntityBase<Guid>
        {
        }

    }
}

