using Autofac;
using Autofac.Builder;
using Autofac.Core;
using Trident.Transactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AF = Autofac;
using Trident.IoC;

namespace Trident.Autofac
{
    /// <summary>
    /// Class AutofacExtension.
    /// </summary>
    public static class AutofacExtension
    {
        /// <summary>
        /// Ases the directly implemented interfaces.
        /// </summary>
        /// <typeparam name="L"></typeparam>
        /// <typeparam name="D"></typeparam>
        /// <typeparam name="S"></typeparam>
        /// <param name="registration">The registration.</param>
        /// <returns>IRegistrationBuilder&lt;L, D, S&gt;.</returns>
        public static IRegistrationBuilder<L, D, S> AsDirectlyImplementedInterfaces<L, D, S>(this IRegistrationBuilder<L, D, S> registration)
            where D : AF.Features.Scanning.ScanningActivatorData
        {
            return registration.As(x => x.GetDirectlyImplementedInterfaces());
        }

        /// <summary>
        /// Registers the named strategies.
        /// </summary>
        /// <typeparam name="TCommonInterface">The type of the t common interface.</typeparam>
        /// <typeparam name="TNameIdentificationAttribute">The type of the t name identification attribute.</typeparam>
        /// <param name="builder">The builder.</param>
        /// <param name="assemblies">The assemblies.</param>
        public static void RegisterNamedStrategies<TCommonInterface, TNameIdentificationAttribute>(this ContainerBuilder builder, Assembly[] assemblies)
        where TNameIdentificationAttribute : Attribute, INamingAttribute
        {
            builder.RegisterAssemblyTypes(assemblies)
              .Where(x => !x.IsAbstract && typeof(TCommonInterface).IsAssignableFrom(x))
              .Named(x =>
              {
                  var attr = x.GetCustomAttribute<TNameIdentificationAttribute>();
                  return (attr != null) ? attr.Name : string.Empty;
              }, typeof(TCommonInterface));
        }

        /// <summary>
        /// Gets all registered instances of T.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="container">The container.</param>
        /// <returns>IEnumerable&lt;T&gt;.</returns>
        public static IEnumerable<T> GetAll<T>(this ILifetimeScope container)
        {
            var results = container.ComponentRegistry.Registrations
              .SelectMany(x => x.Services)
              .OfType<KeyedService>()
              .Where(x => x.ServiceType == typeof(T))
              .Select(t => container.ResolveKeyed<T>(t.ServiceKey));

            return results.Union(container.Resolve<IEnumerable<T>>());
        }

        /// <summary>
        /// Registers the interceptor proxy over classes derived or implementing the TScanInterface in the specified assemblies.
        /// Assumes Executing assembly if non is supplied.
        /// Limitations: Only supports Instance Per lifetime Scopes and doesn't support multiple proxy decorators.
        /// </summary>
        /// <typeparam name="TScanInterface">The type of the interface to use to detect which classes will have proxies applied.</typeparam>
        /// <typeparam name="TProxyFactory">The type of the t proxy factory that will apply the proxy to the derived types.</typeparam>
        /// <param name="builder">The builder.</param>
        /// <param name="assemblies">The assemblies.</param>
        public static void RegisterInterceptorFor<TScanInterface, TProxyFactory>(this ContainerBuilder builder, params Assembly[] assemblies)
            where TScanInterface : class
            where TProxyFactory : IProxyFactory
        {

            assemblies = assemblies ?? new Assembly[] { Assembly.GetExecutingAssembly() };
            //register the Managers as themselves to avoid circular resolution
            builder.RegisterAssemblyTypes(assemblies)
                .Where(x => !x.IsAbstract && typeof(TScanInterface).IsAssignableFrom(x))
                .InstancePerLifetimeScope()
                .AsSelf();

            //scan again and register a factory method that will wrap the mangers with 
            //the EFResilientTransactionProxy
            assemblies.SelectMany(x => x.GetTypes())
                .Where(x => !x.IsAbstract && typeof(TScanInterface).IsAssignableFrom(x))
                .ToList()
                .ForEach(x =>
                {
                    foreach (var z in x.GetDirectlyImplementedInterfaces())
                    {
                        builder.RegisterDynamically(z, y => y.Resolve(x))
                            .InstancePerLifetimeScope()
                            .As(z)
                            .OnActivating(h =>
                            {
                                var ignoreAttr = h.Instance.GetType().GetCustomAttribute<NonTransactionalAttribute>();
                                if (ignoreAttr == null)
                                {
                                    var proxyFactory = h.Context.Resolve<TProxyFactory>();
                                    h.ReplaceInstance(proxyFactory.CreateProxy(z, h.Instance));
                                }
                            });
                    }
                });
        }



        /// <summary>
        /// Registers the interceptor for an individual interface and implementation.
        /// NOTE: No lifetime scope is not applied to the returned Proxy Decorated registration. Use normal autofac life registrations to apply desired scope.
        /// </summary>
        /// <typeparam name="ITargetInterface">The type of the target interface to apply the proxy.</typeparam>
        /// <typeparam name="TImplementation">The type concrete type that contains the implementation of the ITargetInterface.</typeparam>
        /// <typeparam name="TProxyFactory">The type of the proxy factory.</typeparam>
        /// <param name="builder">The builder.</param>
        /// <param name="ConcreteImplementationIsSingleton">if set to <c>true</c> concrete implementation is singleton, otherwise defaults to instance per lifetime scope.</param>
        /// <returns>IRegistrationBuilder&lt;ITargetInterface, SimpleActivatorData, SingleRegistrationStyle&gt;.</returns>
        public static IRegistrationBuilder<ITargetInterface, SimpleActivatorData, SingleRegistrationStyle> RegisterInterceptorFor<ITargetInterface, TImplementation, TProxyFactory>(this ContainerBuilder builder, bool ConcreteImplementationIsSingleton = false)
          where ITargetInterface : class
          where TImplementation : class, ITargetInterface
          where TProxyFactory : IProxyFactory
        {
            builder.RegisterType<TImplementation>()
                .InstancePerLifetimeScope()
                .AsSelf();

            return builder.Register<ITargetInterface>(y => y.Resolve<TImplementation>())
                .InstancePerLifetimeScope()
                .As<ITargetInterface>()
                .OnActivating(h =>
                {
                    var proxyFactory = h.Context.Resolve<TProxyFactory>();
                    h.ReplaceInstance(proxyFactory.CreateProxyGeneric<ITargetInterface>(h.Instance));
                });

        }

        /// <summary>
        /// Registers the delegate and applied interfaces dynamically. Do not expose this method outside this class,
        /// Only used to support assembly scanning registrations for the proxy implementation.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="typeLimit">The type limit.</param>
        /// <param name="delegate">The delegate.</param>
        /// <returns>IRegistrationBuilder&lt;System.Object, SimpleActivatorData, SingleRegistrationStyle&gt;.</returns>
        /// <exception cref="ArgumentNullException">builder
        /// or
        /// delegate</exception>
        private static IRegistrationBuilder<object, SimpleActivatorData, SingleRegistrationStyle>
          RegisterDynamically(
              this ContainerBuilder builder,
              Type typeLimit,
              Func<IComponentContext, object> @delegate)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            if (@delegate == null) throw new ArgumentNullException(nameof(@delegate));

            Func<IComponentContext, IEnumerable<AF.Core.Parameter>, object> @del = (c, p) => @delegate(c);

            var rb = RegistrationBuilder.ForDelegate(typeLimit, @del);

            rb.RegistrationData.DeferredCallback = builder.RegisterCallback(cr => RegistrationBuilder.RegisterSingleComponent(cr, rb));

            return rb;
        }



    }
}
