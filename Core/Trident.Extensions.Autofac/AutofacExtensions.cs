//using Autofac;
//using Autofac.Builder;
//using Autofac.Core;
//using Trident.Transactions;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Reflection;
//using AF = Autofac;
//using Trident.Workflow;
//using Trident.Validation;
//using Trident.Data.Contracts;
//using Trident.Contracts;
//using Trident.Search;
//using Trident.Data;
//using Trident.Business;
//using Trident.Rest;
//using Trident.Rest.Contracts;
//using Trident.Common;
//using Trident.Mapper;
//using Trident.Caching;

//namespace Trident.IoC
//{
//    /// <summary>
//    /// Class AutofacExtension.
//    /// </summary>
//    public static class AutofacExtension
//    {

//        public static void UsingTridentEntityComparer(this ContainerBuilder builder)
//        {
//            builder.RegisterType<EntityComparer>().As<IEntityComparer>().SingleInstance();
//        }

//        public static void UsingTridentFileStorage(this ContainerBuilder builder)
//        {
//            builder.RegisterType<DefaultFileStorageProvider>().As<IFileStorageProvider>().SingleInstance();
//            builder.RegisterType<DefaultFileStorageManager>().As<IFileStorageManager>().SingleInstance();
//        }

//        public static void UsingTridentData(this ContainerBuilder builder)
//        {
//            builder.RegisterType<AbstractContextFactory>().As<IAbstractContextFactory>().InstancePerLifetimeScope();
//            builder.RegisterType<SharedConnectionStringResolver>().As<ISharedConnectionStringResolver>().SingleInstance();
//            builder.RegisterType<RestAuthenticationProviderFactory>().As<IRestAuthenticationFactory>().SingleInstance();
//            builder.RegisterType<RestAuthenticationProviderFactory>().As<IRestAuthenticationFactory>().SingleInstance();
//            builder.RegisterType<RestConnectionStringResolver>().As<IRestConnectionStringResolver>().SingleInstance();
//        }

//        /// <summary>
//        /// Registers the data source dependencies.
//        /// </summary>
//        /// <param name="builder">The builder.</param>
//        /// <param name="targetAssemblies">The target assemblies.</param>
//        public static void UsingTridentSearch(this ContainerBuilder builder, params Assembly[] targetAssemblies)
//        {
//            builder.UsingTridentSearchResultBuilder();
//            builder.UsingTridentSearchQueryBuilder();
//            builder.UsingTridentSearchResultBuilder();
//            builder.UsingTridentSearchComplexFilterFactory();
//            builder.UsingTridentComplexFilters(targetAssemblies);
//            builder.UsingTridentComplexFilterAdapters(targetAssemblies);
//        }

//        public static void UsingTridentMapperProfiles(this ContainerBuilder builder, params Assembly[] targetAssemblies)
//        {
//            builder.RegisterType<ServiceMapperRegistry>().As<IMapperRegistry>().SingleInstance();
//            builder.RegisterAssemblyTypes(targetAssemblies)
//                .Where(x => !x.IsAbstract && typeof(AutoMapper.Profile).IsAssignableFrom(x))
//                .SingleInstance()
//                .As<AutoMapper.Profile>();
//        }

//        public static IRegistrationBuilder<TransactionScopeFactory, ReflectionActivatorData, SingleRegistrationStyle>
//            UsingTridentTransactions(this ContainerBuilder builder)
//        {
//            return builder.RegisterType<TransactionScopeFactory>().As<ITransactionScopeFactory>().SingleInstance();
//        }

//        public static IRegistrationBuilder<InMemoryCachingManager, ReflectionActivatorData, SingleRegistrationStyle>
//            UsingTridentInMemberCachingManager(this ContainerBuilder builder)
//        {
//            return builder.RegisterType<InMemoryCachingManager>().As<ICachingManager>().SingleInstance();
//        }

//        public static IRegistrationBuilder<Object, AF.Features.Scanning.ScanningActivatorData, DynamicRegistrationStyle>
//            UsingTridentValidationManagers(this ContainerBuilder builder, params Assembly[] targetAssemblies)

//        {
//            //this registers the default
//            builder.RegisterGeneric(typeof(DefaultValidationManager<>))
//                 .As(typeof(IValidationManager<>))                 
//                 .InstancePerLifetimeScope()
//                 .AsSelf();          

//            return builder.RegisterAssemblyTypes(targetAssemblies)
//                  .Where(x => !x.IsAbstract && typeof(IValidationManager).IsAssignableFrom(x))
//                  .InstancePerLifetimeScope()
//                  .AsImplementedInterfaces()
//                  .AsSelf();
//        }

//        public static IRegistrationBuilder<Object, AF.Features.Scanning.ScanningActivatorData, DynamicRegistrationStyle>
//            UsingTridentValidationRules(this ContainerBuilder builder, params Assembly[] targetAssemblies)
//        {
//            return builder.RegisterAssemblyTypes(targetAssemblies)
//                .Where(x => !x.IsAbstract && typeof(IValidationRule).IsAssignableFrom(x))
//                .InstancePerLifetimeScope()
//                .AsImplementedInterfaces()
//                .AsSelf();
//        }

//        public static IRegistrationBuilder<Object, AF.Features.Scanning.ScanningActivatorData, DynamicRegistrationStyle>
//            UsingTridentWorkflowManagers(this ContainerBuilder builder, params Assembly[] targetAssemblies)

//        {
//            //this registers the default
//            builder.RegisterGeneric(typeof(DefaultWorkflowManager<>))
//                 .As(typeof(IWorkflowManager<>))
//                 .InstancePerLifetimeScope()
//                 .AsSelf();

//            return builder.RegisterAssemblyTypes(targetAssemblies)
//                 .Where(x => !x.IsAbstract && typeof(IWorkflowManager).IsAssignableFrom(x))
//                 .InstancePerLifetimeScope()
//                 .AsImplementedInterfaces()
//                 .AsSelf();
//        }

//        public static IRegistrationBuilder<Object, AF.Features.Scanning.ScanningActivatorData, DynamicRegistrationStyle>
//            UsingTridentWorkflowTasks(this ContainerBuilder builder, params Assembly[] targetAssemblies)

//        {
//            return builder.RegisterAssemblyTypes(targetAssemblies)
//                 .Where(x => !x.IsAbstract && typeof(IWorkflowTask).IsAssignableFrom(x))
//                 .InstancePerLifetimeScope()
//                 .AsImplementedInterfaces()
//                 .AsSelf();
//        }

//        public static IRegistrationBuilder<object, AF.Features.Scanning.ScanningActivatorData, DynamicRegistrationStyle>
//            UsingTridentProviders(this ContainerBuilder builder, params Assembly[] targetAssemblies)

//        {
//            return builder.RegisterAssemblyTypes(targetAssemblies)
//                .Where(x => !x.IsAbstract && typeof(IProvider).IsAssignableFrom(x))
//                .InstancePerLifetimeScope()
//                .AsImplementedInterfaces()
//                .AsSelf();
//        }


//        public static IRegistrationBuilder<object, AF.Features.Scanning.ScanningActivatorData, DynamicRegistrationStyle>
//            UsingTridentManagers(this ContainerBuilder builder, params Assembly[] targetAssemblies)

//        {
//            return builder.RegisterAssemblyTypes(targetAssemblies)
//                 .Where(x => !x.IsAbstract && typeof(IManager).IsAssignableFrom(x))
//                 .InstancePerLifetimeScope()
//                 .AsImplementedInterfaces()
//                 .AsSelf();
//        }



//        public static IRegistrationBuilder<object, AF.Features.Scanning.ScanningActivatorData, DynamicRegistrationStyle>
//            UsingTridentRepositories(this ContainerBuilder builder, params Assembly[] targetAssemblies)

//        {
//            return builder.RegisterAssemblyTypes(targetAssemblies)
//                .Where(x => !x.IsAbstract && typeof(IRepositoryBase).IsAssignableFrom(x))
//                .InstancePerLifetimeScope()
//                .AsImplementedInterfaces()
//                .AsSelf();
//        }  

//        public static IRegistrationBuilder<SearchResultsBuilder, ReflectionActivatorData, SingleRegistrationStyle> 
//            UsingTridentSearchResultBuilder(this ContainerBuilder builder)
//        {
//            return builder.RegisterType<SearchResultsBuilder>().As<ISearchResultsBuilder>().InstancePerLifetimeScope();        
//        }

//        public static IRegistrationBuilder<SearchQueryBuilder, ReflectionActivatorData, SingleRegistrationStyle> 
//            UsingTridentSearchQueryBuilder(this ContainerBuilder builder)
//        {     
//            return builder.RegisterType<SearchQueryBuilder>().As<ISearchQueryBuilder>().InstancePerLifetimeScope();          
//        }


//        public static IRegistrationBuilder<ComplexFilterFactory, ReflectionActivatorData, SingleRegistrationStyle> 
//            UsingTridentSearchComplexFilterFactory(this ContainerBuilder builder)
//        {
//            return builder.RegisterType<ComplexFilterFactory>().As<IComplexFilterFactory>().InstancePerLifetimeScope();          
//        }


//        public static IRegistrationBuilder<object, AF.Features.Scanning.ScanningActivatorData, DynamicRegistrationStyle>
//            UsingTridentComplexFilters(this ContainerBuilder builder, params Assembly[] targetAssemblies)
//        {
//            return builder.RegisterAssemblyTypes(targetAssemblies)
//                .Where(x => !x.IsAbstract && typeof(IComplexFilter).IsAssignableFrom(x))
//                .As<IComplexFilter>()
//                .InstancePerLifetimeScope()
//                .AsSelf();
//        }

//        public static IRegistrationBuilder<object, AF.Features.Scanning.ScanningActivatorData, DynamicRegistrationStyle>
//            UsingTridentComplexFilterAdapters(this ContainerBuilder builder, params Assembly[] targetAssemblies)
//        {

//            return builder.RegisterAssemblyTypes(targetAssemblies)
//             .Where(x => !x.IsAbstract && typeof(IComplexFilterAdapter).IsAssignableFrom(x))
//             .As<IComplexFilterAdapter>()
//             .InstancePerLifetimeScope()
//             .AsSelf();
//        }



//        public static IRegistrationBuilder<object, AF.Features.Scanning.ScanningActivatorData, DynamicRegistrationStyle>
//            UsingTridentStrategy<T>(this ContainerBuilder builder, params Assembly[] targetAssemblies)

//        {
//            return builder.RegisterAssemblyTypes(targetAssemblies)
//               .Where(x => !x.IsAbstract && typeof(T).IsAssignableFrom(x))
//               .InstancePerLifetimeScope()
//               .As<T>()
//               .AsSelf();
//        }


//        public static IRegistrationBuilder<Common.IAppSettings, ConcreteReflectionActivatorData, SingleRegistrationStyle>
//            UsingTridentAppSettingsXmlManager(this ContainerBuilder builder)

//        {
//            return builder
//                .RegisterType<Common.XmlAppSettings>()
//                .As<Common.IAppSettings>()
//                .SingleInstance()
//                .AsSelf();
//        }

//        public static IRegistrationBuilder<Common.IAppSettings, ConcreteReflectionActivatorData, SingleRegistrationStyle>
//          UsingTridentAppSettingsJsonManager(this ContainerBuilder builder)

//        {
//            return builder
//                .RegisterType<Common.JsonAppSettings>()
//                .As<Common.IAppSettings>()
//                .SingleInstance()
//                .AsSelf();
//        }     

//        public static IRegistrationBuilder<IConnectionStringSettings, ConcreteReflectionActivatorData, SingleRegistrationStyle>
//          UsingTridentConnectionStringXmlManager(this ContainerBuilder builder)
//        {
//            return builder
//                .RegisterType<XmlConnectionStringSettings>()
//                .As<IConnectionStringSettings>()
//                .SingleInstance()
//                .AsSelf();
//        }

//        public static IRegistrationBuilder<IConnectionStringSettings, ConcreteReflectionActivatorData, SingleRegistrationStyle>
//        UsingTridentConnectionStringJsonManager(this ContainerBuilder builder)
//        {
//            return builder
//                .RegisterType<JsonConnectionStringSettings>()
//                .As<IConnectionStringSettings>()
//                .SingleInstance()
//                .AsSelf();
//        }


//        /// <summary>
//        /// Ases the directly implemented interfaces.
//        /// </summary>
//        /// <typeparam name="L"></typeparam>
//        /// <typeparam name="D"></typeparam>
//        /// <typeparam name="S"></typeparam>
//        /// <param name="registration">The registration.</param>
//        /// <returns>IRegistrationBuilder&lt;L, D, S&gt;.</returns>
//        public static IRegistrationBuilder<L, D, S> AsDirectlyImplementedInterfaces<L, D, S>(this IRegistrationBuilder<L, D, S> registration)
//            where D : AF.Features.Scanning.ScanningActivatorData
//        {
//            return registration.As(x => x.GetDirectlyImplementedInterfaces());
//        }

//        /// <summary>
//        /// Registers the named strategies.
//        /// </summary>
//        /// <typeparam name="TCommonInterface">The type of the t common interface.</typeparam>
//        /// <typeparam name="TNameIdentificationAttribute">The type of the t name identification attribute.</typeparam>
//        /// <param name="builder">The builder.</param>
//        /// <param name="assemblies">The assemblies.</param>
//        public static void RegisterNamedStrategies<TCommonInterface, TNameIdentificationAttribute>(this ContainerBuilder builder, Assembly[] assemblies)
//        where TNameIdentificationAttribute : Attribute, INamingAttribute
//        {
//            builder.RegisterAssemblyTypes(assemblies)
//              .Where(x => !x.IsAbstract && typeof(TCommonInterface).IsAssignableFrom(x))
//              .Named(x =>
//              {
//                  var attr = x.GetCustomAttribute<TNameIdentificationAttribute>();
//                  return (attr != null) ? attr.Name : string.Empty;
//              }, typeof(TCommonInterface));
//        }

//        /// <summary>
//        /// Gets all registered instances of T.
//        /// </summary>
//        /// <typeparam name="T"></typeparam>
//        /// <param name="container">The container.</param>
//        /// <returns>IEnumerable&lt;T&gt;.</returns>
//        public static IEnumerable<T> GetAll<T>(this ILifetimeScope container)
//        {
//            var results = container.ComponentRegistry.Registrations
//              .SelectMany(x => x.Services)
//              .OfType<KeyedService>()
//              .Where(x => x.ServiceType == typeof(T))
//              .Select(t => container.ResolveKeyed<T>(t.ServiceKey));

//            return results.Union(container.Resolve<IEnumerable<T>>());
//        }

//        /// <summary>
//        /// Registers the interceptor proxy over classes derived or implementing the TScanInterface in the specified assemblies.
//        /// Assumes Executing assembly if non is supplied.
//        /// Limitations: Only supports Instance Per lifetime Scopes and doesn't support multiple proxy decorators.
//        /// </summary>
//        /// <typeparam name="TScanInterface">The type of the interface to use to detect which classes will have proxies applied.</typeparam>
//        /// <typeparam name="TProxyFactory">The type of the t proxy factory that will apply the proxy to the derived types.</typeparam>
//        /// <param name="builder">The builder.</param>
//        /// <param name="assemblies">The assemblies.</param>
//        public static void RegisterInterceptorFor<TScanInterface, TProxyFactory>(this ContainerBuilder builder, params Assembly[] assemblies)
//            where TScanInterface : class
//            where TProxyFactory : IProxyFactory
//        {

//            assemblies = assemblies ?? new Assembly[] { Assembly.GetExecutingAssembly() };
//            //register the Managers as themselves to avoid circular resolution
//            builder.RegisterAssemblyTypes(assemblies)
//                .Where(x => !x.IsAbstract && typeof(TScanInterface).IsAssignableFrom(x))
//                .InstancePerLifetimeScope()
//                .AsSelf();

//            //scan again and register a factory method that will wrap the mangers with 
//            //the EFResilientTransactionProxy
//            assemblies.SelectMany(x => x.GetTypes())
//                .Where(x => !x.IsAbstract && typeof(TScanInterface).IsAssignableFrom(x))
//                .ToList()
//                .ForEach(x =>
//                {
//                    foreach (var z in x.GetDirectlyImplementedInterfaces())
//                    {
//                        builder.RegisterDynamically(z, y => y.Resolve(x))
//                            .InstancePerLifetimeScope()
//                            .As(z)
//                            .OnActivating(h =>
//                            {
//                                var ignoreAttr = h.Instance.GetType().GetCustomAttribute<NonTransactionalAttribute>();
//                                if (ignoreAttr == null)
//                                {
//                                    var proxyFactory = h.Context.Resolve<TProxyFactory>();
//                                    h.ReplaceInstance(proxyFactory.CreateProxy(z, h.Instance));
//                                }
//                            });
//                    }
//                });
//        }



//        /// <summary>
//        /// Registers the interceptor for an individual interface and implementation.
//        /// NOTE: No lifetime scope is not applied to the returned Proxy Decorated registration. Use normal autofac life registrations to apply desired scope.
//        /// </summary>
//        /// <typeparam name="ITargetInterface">The type of the target interface to apply the proxy.</typeparam>
//        /// <typeparam name="TImplementation">The type concrete type that contains the implementation of the ITargetInterface.</typeparam>
//        /// <typeparam name="TProxyFactory">The type of the proxy factory.</typeparam>
//        /// <param name="builder">The builder.</param>
//        /// <param name="ConcreteImplementationIsSingleton">if set to <c>true</c> concrete implementation is singleton, otherwise defaults to instance per lifetime scope.</param>
//        /// <returns>IRegistrationBuilder&lt;ITargetInterface, SimpleActivatorData, SingleRegistrationStyle&gt;.</returns>
//        public static IRegistrationBuilder<ITargetInterface, SimpleActivatorData, SingleRegistrationStyle> RegisterInterceptorFor<ITargetInterface, TImplementation, TProxyFactory>(this ContainerBuilder builder, bool ConcreteImplementationIsSingleton = false)
//          where ITargetInterface : class
//          where TImplementation : class, ITargetInterface
//          where TProxyFactory : IProxyFactory
//        {
//            builder.RegisterType<TImplementation>()
//                .InstancePerLifetimeScope()
//                .AsSelf();

//            return builder.Register<ITargetInterface>(y => y.Resolve<TImplementation>())
//                .InstancePerLifetimeScope()
//                .As<ITargetInterface>()
//                .OnActivating(h =>
//                {
//                    var proxyFactory = h.Context.Resolve<TProxyFactory>();
//                    h.ReplaceInstance(proxyFactory.CreateProxyGeneric<ITargetInterface>(h.Instance));
//                });

//        }

//        /// <summary>
//        /// Registers the delegate and applied interfaces dynamically. Do not expose this method outside this class,
//        /// Only used to support assembly scanning registrations for the proxy implementation.
//        /// </summary>
//        /// <param name="builder">The builder.</param>
//        /// <param name="typeLimit">The type limit.</param>
//        /// <param name="delegate">The delegate.</param>
//        /// <returns>IRegistrationBuilder&lt;System.Object, SimpleActivatorData, SingleRegistrationStyle&gt;.</returns>
//        /// <exception cref="ArgumentNullException">builder
//        /// or
//        /// delegate</exception>
//        private static IRegistrationBuilder<object, SimpleActivatorData, SingleRegistrationStyle>
//          RegisterDynamically(
//              this ContainerBuilder builder,
//              Type typeLimit,
//              Func<IComponentContext, object> @delegate)
//        {
//            if (builder == null) throw new ArgumentNullException(nameof(builder));
//            if (@delegate == null) throw new ArgumentNullException(nameof(@delegate));

//            Func<IComponentContext, IEnumerable<AF.Core.Parameter>, object> @del = (c, p) => @delegate(c);

//            var rb = RegistrationBuilder.ForDelegate(typeLimit, @del);

//            rb.RegistrationData.DeferredCallback = builder.RegisterCallback(cr => RegistrationBuilder.RegisterSingleComponent(cr, rb));

//            return rb;
//        }



//    }
//}
