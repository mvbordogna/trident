﻿using SexyProxy;
using Trident.IoC;
using Trident.Transactions;
using System;
using System.Collections.Concurrent;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Reflection;

namespace Trident.EF6
{

    /// <summary>
    /// Interface IEFResilientAsyncTransactionProxyFactory
    /// Implements the <see cref="Trident.IProxyFactory" />
    /// </summary>
    /// <seealso cref="Trident.IProxyFactory" />
    public interface IEFResilientAsyncTransactionProxyFactory : IProxyFactory { }

    /// <summary>
    /// Class EFResilientAsyncTransactionProxyFactory.
    /// </summary>
    /// <seealso cref="Trident.EF6.IEFResilientAsyncTransactionProxyFactory" />
    public class EFResilientAsyncTransactionProxyFactory : IEFResilientAsyncTransactionProxyFactory
    {
        /// <summary>
        /// The transaction scope factory
        /// </summary>
        private readonly ITransactionScopeFactory _transactionScopeFactory;
        /// <summary>
        /// The generic methods
        /// </summary>
        private readonly ConcurrentDictionary<Type, MethodInfo> _genericMethods = new ConcurrentDictionary<Type, MethodInfo>();

        /// <summary>
        /// Initializes a new instance of the <see cref="EFResilientAsyncTransactionProxyFactory" /> class.
        /// </summary>
        /// <param name="transactionScopeFactory">The transaction scope factory.</param>
        public EFResilientAsyncTransactionProxyFactory(ITransactionScopeFactory transactionScopeFactory)
        {
            _transactionScopeFactory = transactionScopeFactory;
        }

        /// <summary>
        /// Creates the proxy.
        /// </summary>
        /// <param name="limitType">Type of the limit.</param>
        /// <param name="target">The target.</param>
        /// <returns>System.Object.</returns>
        public object CreateProxy(Type limitType, object target)
        {
            var typedGenMethod = _genericMethods.GetOrAdd(limitType, (t) =>
           {
               var genMethodInfo = this.GetType().GetMethod("CreateProxyGeneric");
               return genMethodInfo.MakeGenericMethod(limitType);
           });

            return typedGenMethod.Invoke(this, new object[] { target });
        }

        /// <summary>
        /// Creates the proxy generic.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target">The target.</param>
        /// <returns>T.</returns>
        public T CreateProxyGeneric<T>(T target) where T : class
        {
            return Proxy.CreateProxy(target, async invocation =>
            {
                var flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy;
                var targetMethod = target.GetType().GetMember(invocation.Method.Name, flags).First();
                if (IsMarkedNonTransactional(invocation, targetMethod))
                {
                    return await invocation.Proceed();
                }
                else
                {
                    object result = null;

                    if (DynamicExecutionStrategyDbConfiguration.SuspendExecutionStrategy)
                    {
                        using (var trans = _transactionScopeFactory.GetTransactionScope())
                        {
                            result = await invocation.Proceed();
                            trans.Complete();
                        }
                    }
                    else
                    {

                        DynamicExecutionStrategyDbConfiguration.SuspendExecutionStrategy = true;
                        var executionStrategy = new SqlAzureExecutionStrategy(3, TimeSpan.FromMilliseconds(200));

                        await executionStrategy.ExecuteAsync(async () =>
                        {
                            using (var trans = _transactionScopeFactory.GetTransactionScope())
                            {
                                result = await invocation.Proceed();
                                trans.Complete();
                            }
                        }, new System.Threading.CancellationToken());

                        DynamicExecutionStrategyDbConfiguration.SuspendExecutionStrategy = false;
                    }

                    return result;
                }
            });
        }

        /// <summary>
        /// Determines whether [is marked non transactional] [the specified invocation].
        /// </summary>
        /// <param name="invocation">The invocation.</param>
        /// <param name="targetMethod">The target method.</param>
        /// <returns><c>true</c> if [is marked non transactional] [the specified invocation]; otherwise, <c>false</c>.</returns>
        private bool IsMarkedNonTransactional(Invocation invocation, MemberInfo targetMethod)
        {
            return invocation.Method.GetCustomAttribute<NonTransactionalAttribute>() != null //on interface method
              || invocation.Method.DeclaringType.GetCustomAttribute<NonTransactionalAttribute>() != null //on interface
              || targetMethod.GetCustomAttribute<NonTransactionalAttribute>() != null //on class method
              || targetMethod.DeclaringType.GetCustomAttribute<NonTransactionalAttribute>() != null; //on class
        }
    }
}