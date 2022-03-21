using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Trident.Data.Contracts
{
    public interface IQueryableHelper
    {
        Task<T> FirstOrDefaultAsync<T>(IContext context, IQueryable<T> queryable, Expression<Func<T, bool>> idExpression, bool detach = false) where T : class;
        T FirstOrDefault<T>(IContext context, IQueryable<T> queryable, Expression<Func<T, bool>> idExpression, bool detach = false) where T : class;
        Task<List<T>> ToListAsync<T>(IContext context, IQueryable<T> queryable, bool detach = false) where T : class;
        List<T> ToList<T>(IContext context, IQueryable<T> queryable, bool detach = false) where T : class;
        Task<bool> AnyAsync<T>(IQueryable<T> queryable, Expression<Func<T, bool>> filter) where T : class;
        IQueryable<T> Include<T>(IQueryable<T> queryable, string includeProperty) where T : class;
        Task<int> CountAsync<T>(IQueryable<T> queryable) where T : class;
        IQueryable<T> AsNoTracking<T>(IQueryable<T> queryable) where T : class;
    }
}
