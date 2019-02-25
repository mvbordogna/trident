using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trident.Data.EntityFramework
{
    /// <summary>
    /// Class ContextInitializerNone.
    /// Implements the <see cref="System.Data.Entity.IDatabaseInitializer{TContext}" />
    /// </summary>
    /// <typeparam name="TContext">The type of the t context.</typeparam>
    /// <seealso cref="System.Data.Entity.IDatabaseInitializer{TContext}" />
    public class ContextInitializerNone<TContext> : IDatabaseInitializer<TContext> where TContext : DbContext
    {
        /// <summary>
        /// Executes the strategy to initialize the database for the given context.
        /// </summary>
        /// <param name="context">The context.</param>
        public void InitializeDatabase(TContext context) { }
    }
}
