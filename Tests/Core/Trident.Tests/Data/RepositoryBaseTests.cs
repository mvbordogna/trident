using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Trident.Testing.TestScopes;
using Trident.Data;
using Trident.Data.Contracts;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Trident.Testing;
using System.Threading;

namespace Trident.Tests.Data
{
    [TestClass]
    public class RepositoryBaseTests
    {
        [TestMethod]
        [TestCategory(TestCategory.Unit)]
        public async Task GetById_Delegates_To_Context()
        {
            // Arrange
            var scope = new DefaultScope();
            var id = Guid.NewGuid();

            // Act

            await scope.InstanceUnderTest.GetById(id);

            // Assert          
            scope.ContextMock.Verify(x => x.FindAsync<MockEntity>(id, false), Times.Once());
        }

        [TestMethod]
        [TestCategory(TestCategory.Unit)]
        public async Task Insert_DefersCommit()
        {
            // Arrange
            var scope = new DefaultScope();
            var entity = new MockEntity();

            // Act

            await scope.InstanceUnderTest.Insert(entity, true);


            // Assert
            scope.ContextMock.Verify(x => x.Add<MockEntity>(entity), Times.Once());
            scope.ContextMock.Verify(x => x.SaveChangesAsync(default(CancellationToken)), Times.Never());
        }

        [TestMethod]
        [TestCategory(TestCategory.Unit)]
        public async Task Insert_Commits()
        {
            // Arrange
            var scope = new DefaultScope();
            var entity = new MockEntity();

            // Act

            await scope.InstanceUnderTest.Insert(entity, false);

            // Assert
            scope.ContextMock.Verify(x => x.Add<MockEntity>(entity), Times.Once());
            scope.ContextMock.Verify(x => x.SaveChangesAsync(default(CancellationToken)), Times.Once());
        }

        [TestMethod]
        [TestCategory(TestCategory.Unit)]
        public async Task Delete_DefersCommit()
        {
            // Arrange
            var scope = new DefaultScope();
            var entity = new MockEntity();

            // Act
            await scope.InstanceUnderTest.Delete(entity, true);

            // Assert
            scope.ContextMock.Verify(x => x.Delete<MockEntity>(entity), Times.Once());
            scope.ContextMock.Verify(x => x.SaveChangesAsync(default(CancellationToken)), Times.Never());
        }

        [TestMethod]
        [TestCategory(TestCategory.Unit)]
        public async Task Delete_Commits()
        {
            // Arrange
            var scope = new DefaultScope();
            var entity = new MockEntity();

            // Act

            await scope.InstanceUnderTest.Delete(entity, false);


            // Assert
            scope.ContextMock.Verify(x => x.Delete<MockEntity>(entity), Times.Once());
            scope.ContextMock.Verify(x => x.SaveChangesAsync(default(CancellationToken)), Times.Once());
        }

        [TestMethod]
        [TestCategory(TestCategory.Unit)]
        public async Task Update_DefersCommit()
        {
            // Arrange
            var scope = new DefaultScope();
            var entity = new MockEntity();

            // Act

            await scope.InstanceUnderTest.Update(entity, true);

            // Assert
            scope.ContextMock.Verify(x => x.Update<MockEntity>(entity), Times.Once());
            scope.ContextMock.Verify(x => x.SaveChangesAsync(default(CancellationToken)), Times.Never());
        }

        [TestMethod]
        [TestCategory(TestCategory.Unit)]
        public async Task Update_Commits()
        {
            // Arrange
            var scope = new DefaultScope();
            var entity = new MockEntity();

            // Act

            await scope.InstanceUnderTest.Update(entity, false);

            // Assert
            scope.ContextMock.Verify(x => x.Update<MockEntity>(entity), Times.Once());
            scope.ContextMock.Verify(x => x.SaveChangesAsync(default(CancellationToken)), Times.Once());
        }

        [TestMethod]
        [TestCategory(TestCategory.Unit)]
        public async Task Commit_Delegates()
        {
            // Arrange
            var scope = new DefaultScope();

            // Act           
            await scope.InstanceUnderTest.Commit();

            // Assert
            scope.ContextMock.Verify(x => x.SaveChangesAsync(default(CancellationToken)), Times.Once());
        }


        private class DefaultScope : TestScope<RepositoryBaseConcrete>
        {
            public Mock<IContext> ContextMock { get; }

            public DefaultScope()
            {
                ContextMock = new Mock<IContext>();
                InstanceUnderTest = new RepositoryBaseConcrete(ContextMock.Object);
            }
        }

        private class RepositoryBaseConcrete : RepositoryBase<MockEntity>
        {
            public RepositoryBaseConcrete(IContext context) : base(new Lazy<IContext>(() => context)) { }

            public override Task<bool> Exists(Expression<Func<MockEntity, bool>> filter)
            {
                throw new NotImplementedException();
            }

            public override bool ExistSync(Expression<Func<MockEntity, bool>> filter)
            {
                throw new NotImplementedException();
            }

            public override Task<IEnumerable<MockEntity>> Get(Expression<Func<MockEntity, bool>> filter = null, Func<IQueryable<MockEntity>, IOrderedQueryable<MockEntity>> orderBy = null, IEnumerable<string> includeProperties = null, bool noTracking = false)
            {
                throw new NotImplementedException();
            }

            public override Task<IEnumerable<MockEntity>> GetByIds<TId>(IEnumerable<TId> ids, bool detach = false)
            {
                throw new NotImplementedException();
            }

            public override IEnumerable<MockEntity> GetByIdsSync<TEntityId>(IEnumerable<TEntityId> ids, bool detach = false)
            {
                throw new NotImplementedException();
            }

            public override IEnumerable<MockEntity> GetSync(Expression<Func<MockEntity, bool>> filter = null, Func<IQueryable<MockEntity>, IOrderedQueryable<MockEntity>> orderBy = null, IEnumerable<string> includeProperties = null, bool noTracking = false)
            {
                throw new NotImplementedException();
            }
        }
        public class MockEntity
        {

        }
    }
}
