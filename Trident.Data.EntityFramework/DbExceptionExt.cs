using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;

namespace Trident.Data.EntityFramework
{
    /// <summary>
    /// Class DbExceptionExt.
    /// </summary>
    public static class DbExceptionExt
    {
        /// <summary>
        /// Checks if the given DbUpdateException is a unique constraint violation.
        /// </summary>
        /// <param name="e">The e.</param>
        /// <returns><c>true</c> if [is unique constraint violation] [the specified e]; otherwise, <c>false</c>.</returns>
        public static bool IsUniqueConstraintViolation(this DbUpdateException e)
        {
            return e.InnerException?.InnerException is SqlException sqlEx && (sqlEx.Number == 2601 || sqlEx.Number == 2627);
        }
    }
}
