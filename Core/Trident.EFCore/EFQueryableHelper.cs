namespace Trident.EFCore
{
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using Trident.Data.Contracts;

    public class EFQueryableHelper : IQueryableHelper
    {
        public Task<bool> AnyAsync<T>(IQueryable<T> queryable, Expression<Func<T, bool>> filter) where T : class
        {
            return queryable.AnyAsync<T>(filter);
        }

        public IQueryable<T> AsNoTracking<T>(IQueryable<T> queryable) where T : class
        {
            return queryable.AsNoTracking<T>();
        }

        public Task<int> CountAsync<T>(IQueryable<T> queryable) where T : class
        {
            return queryable.CountAsync();
        }

        public async Task<T> FirstOrDefaultAsync<T>(IContext context, IQueryable<T> queryable, Expression<Func<T, bool>> idExpression, bool detach = false) where T : class
        {
            var q = context.Query<T>(detach);
            var result = await queryable.FirstOrDefaultAsync<T>(idExpression);
            if (!detach)
            {
                context.MapDynamicObjects(result);
            }
            return result;
        }

        public T FirstOrDefault<T>(IContext context, IQueryable<T> queryable, Expression<Func<T, bool>> idExpression, bool detach = false) where T : class
        {
            var q = context.Query<T>(detach);
            var result = queryable.FirstOrDefault<T>(idExpression);
            if (!detach)
            {
                context.MapDynamicObjects(result);
            }
            return result;
        }

        public IQueryable<T> Include<T>(IQueryable<T> queryable, string includeProperty) where T : class
        {
            return queryable.Include(includeProperty);
        }

        public List<T> ToList<T>(IContext context, IQueryable<T> queryable, bool detach = false) where T : class
        {
            var results = queryable.ToList<T>();

            if (!detach)
            {
                foreach (var result in results)
                {
                    context.MapDynamicObjects(result);
                }
            }

            return results;
        }

        public async Task<List<T>> ToListAsync<T>(IContext context, IQueryable<T> queryable, bool detach = false) where T : class
        {
            var results = await queryable.ToListAsync<T>();

            if (!detach)
            {
                foreach (var result in results)
                {
                    context.MapDynamicObjects(result);
                }
            }

            return results;
        }
    }


}
