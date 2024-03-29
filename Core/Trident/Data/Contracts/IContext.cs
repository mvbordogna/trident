﻿using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Trident.Data.Contracts
{
    /// <summary>
    /// Interface IContext
    /// </summary>
    public interface IContext
    {
        /// <summary>
        /// Adds the specified entity.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity">The entity.</param>
        void Add<T>(T entity) where T : class;
        /// <summary>
        /// Deletes the specified entity.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity">The entity.</param>
        void Delete<T>(T entity) where T : class;
        /// <summary>
        /// Updates the specified entity.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity">The entity.</param>
        void Update<T>(T entity) where T : class;


        T Find<T>(object id, bool detach = false) where T : class;

        /// <summary>
        /// Finds the asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id">The identifier.</param>
        /// <param name="detach">if set to <c>true</c> [detach].</param>
        /// <returns>Task&lt;T&gt;.</returns>
        Task<T> FindAsync<T>(object id, bool detach=false) where T : class;               
        /// <summary>
        /// Queries the specified no tracking.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="noTracking">if set to <c>true</c> [no tracking].</param>
        /// <returns>IQueryable&lt;T&gt;.</returns>
        IQueryable<T> Query<T>(bool noTracking=false) where T : class;

        IQueryable<T> ExecuteProcedure<T>(string procedureName, bool noTracking = false, params IDbDataParameter[] parameters) where T : class;

        /// <summary>
        /// Maps data from the raw JObject to any DynmicOjbectAttribute Marked members
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="result"></param>
        void MapDynamicObjects<TEntity>(TEntity result) where TEntity : class;

        /// <summary>
        /// Executes the procedure asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="noTracking">if set to <c>true</c> [no tracking].</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>Task&lt;IQueryable&lt;T&gt;&gt;.</returns>
        Task<IQueryable<T>> ExecuteProcedureAsync<T>(string procedureName, bool noTracking = false, params IDbDataParameter[] parameters) where T : class;

        int ExecuteNonQuery(string command, params object[] parameters);
        /// <summary>
        /// Executes the non query asynchronous.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>Task&lt;System.Int32&gt;.</returns>
        Task<int> ExecuteNonQueryAsync(string command, params object[] parameters);
        /// <summary>
        /// Saves the changes asynchronous.
        /// </summary>
        /// <param name="cancellationToken">Converts to ken.</param>
        /// <returns>Task&lt;System.Int32&gt;.</returns>
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Saves the changes asynchronous and updated any members marked with the DynamicObjetAttribute for the specific Entity type.
        /// </summary>
        /// <param name="cancellationToken">Converts to ken.</param>
        /// <returns>Task&lt;System.Int32&gt;.</returns>
        /// <remarks>This mehtod doesn't work across multiple db sets</remarks>
        Task<int> SaveChangesAsync<TEntity>(CancellationToken cancellationToken = default) where TEntity : class;

        /// <summary>
        /// Saves the changes.
        /// </summary>
        /// <returns>System.Int32.</returns>
        int SaveChanges();

        /// <summary>
        /// Gets the database provider underlying client
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        IDbClient<T> GetDbClient<T>();

        /// <summary>
        /// Executes the specified query for the type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="command"></param>
        /// <param name="parameters"></param>
        /// <returns>Returns an IEnumberable of T</returns>
        Task<IEnumerable<TEntity>> ExecuteQueryAsync<TEntity>(string command, IDictionary<string, object> parameters = null) where TEntity :class;
    }
}
