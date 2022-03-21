using Autofac;
using Autofac.Configuration;
using Autofac.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using Trident.Extensions;
using Microsoft.Extensions.Configuration;
using System.Reflection;
using Autofac.Builder;
using Autofac.Features.Scanning;
using Microsoft.Extensions.DependencyInjection;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Trident.IoC
{
    /// <summary>
    /// Class IoCProvider.
    /// Implements the <see cref="Trident.IIoCProvider" />
    /// </summary>
    /// <seealso cref="Trident.IIoCProvider" />
    public class AutofacIoCProvider : IIoCProvider
    {
        /// <summary>
        /// The builder
        /// </summary>
        private readonly ContainerBuilder _builder;
        /// <summary>
        /// The container
        /// </summary>
        private ILifetimeScope _container;
        /// <summary>
        /// The is initializing
        /// </summary>
        private bool isInitializing = true;
        /// <summary>
        /// The instance
        /// </summary>
        private static IIoCProvider instance = null;
        private bool _disposed;

        /// <summary>
        /// The pad lock
        /// </summary>
        private static readonly object pad_lock = new object();

        /// <summary>
        /// Initializes a new instance of the <see cref="AutofacIoCProvider"/> class.
        /// </summary>
        /// <param name="builder">The builder.</param>
        public AutofacIoCProvider(ContainerBuilder builder)
        {
            _builder = builder;
        }

        private AutofacIoCProvider(ILifetimeScope builder)
        {
            _container = builder;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AutofacIoCProvider"/> class.
        /// </summary>
        public AutofacIoCProvider() : this(new ContainerBuilder()) { }


        /// <summary>
        /// Gets a static global reference to the IoCProvider
        /// This should not be used anywhere besides the legacy code.
        /// This was converted to singleton to support the static service locator
        /// pattern that was used throughout the application
        /// </summary>
        /// <value>The global.</value>
        public static IIoCProvider Global
        {
            get
            {
                if (instance == null)
                {
                    lock (pad_lock)
                    {
                        if (instance == null)
                        {
                            instance = new AutofacIoCProvider();
                        }
                    }
                }

                return instance;
            }
        }

        /// <summary>
        /// Gets the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>System.Object.</returns>
        public object Get(Type type)
        {
            return _container.Resolve(type);
        }

        public object GetOptional(Type type)
        {
            return _container.ResolveOptional(type);
        }


        /// <summary>
        /// Gets this instance.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>T.</returns>
        public T Get<T>()
        {
            return _container.Resolve<T>();
        }

        public bool TryGet<T>(out T service)
        {
            service = default(T);
            var temp = _container.ResolveOptional(typeof(T));
            if (temp != null)
            {
                service = (T)temp;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Gets the specified parameters.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parameters">The parameters.</param>
        /// <returns>T.</returns>
        public T Get<T>(params IoC.Parameter[] parameters)
        {
            var autofacTypeParameters = parameters
                .Select(x => new TypedParameter(x.ParameterType, x.Value));

            return _container.Resolve<T>(autofacTypeParameters);
        }

        /// <summary>
        /// Gets the builder.
        /// </summary>
        /// <value>The builder.</value>
        public ContainerBuilder Builder
        {
            get
            {
                return (isInitializing) ? _builder : null;
            }
        }

        /// <summary>
        /// Resolves all keyed.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>IEnumerable&lt;T&gt;.</returns>
        public IEnumerable<T> ResolveAllKeyed<T>()
        {
            return _container.ComponentRegistry.Registrations
              .SelectMany(x => x.Services)
              .OfType<KeyedService>()
              .Where(x => x.ServiceType == typeof(T))
              .Select(t => _container.ResolveKeyed<T>(t.ServiceKey));
        }


        /// <summary>
        /// Registers the module.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>IIoCProvider.</returns>
        public IIoCProvider RegisterModule<T>()
            where T : Autofac.Module, new()
        {
            _builder.RegisterModule<T>();
            return this;
        }


        /// <summary>
        /// Registers the module.
        /// </summary>
        /// <param name="moduleType">Type of the module.</param>
        /// <returns>IIoCProvider.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">The Type provider {moduleType.FullName} must implement {typeof(IModule).FullName}</exception>
        public IIoCProvider RegisterModule(Type moduleType)
        {
            moduleType.GuardIsNotNull(nameof(moduleType));
            var instance = (IModule)Activator.CreateInstance(moduleType);
            instance.GuardIsNotNull(nameof(instance));
            instance.Configure(this);
            return this;
        }

        /// <summary>
        /// Resolves all typed.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>IEnumerable&lt;T&gt;.</returns>
        public IEnumerable<T> ResolveAllTyped<T>()
        {
            return _container.Resolve<IEnumerable<T>>();
        }

        public IIoCProvider RegisterAll<T>(Assembly[] targetAssemblies, LifeSpan lifeSpan = LifeSpan.InstancePerLifetimeScope)
        {
            var temp = _builder.RegisterAssemblyTypes(targetAssemblies)
                 .Where(x => !x.IsAbstract && typeof(T).IsAssignableFrom(x))
                 .AsImplementedInterfaces();

            ApplyLifeTime(temp, lifeSpan);
            return this;
        }


        /// <summary>
        /// Registers the behavior.
        /// </summary>
        /// <typeparam name="InterfaceOfT">The type of the interface of t.</typeparam>
        /// <param name="constructionFunc">The construction function.</param>
        /// <returns>IIoCProvider.</returns>
        public IIoCProvider RegisterBehavior<InterfaceOfT>(Func<InterfaceOfT> constructionFunc, LifeSpan lifeSpan = LifeSpan.InstancePerLifetimeScope)
        {
            var x = _builder.Register(c =>
            {
                return constructionFunc();

            }).As<InterfaceOfT>();

            ApplyLifeTime(x, lifeSpan);
            return this;
        }


        public IIoCProvider RegisterBehavior<InterfaceOfT>(Func<InterfaceOfT> constructionFunc, string serviceName, LifeSpan lifeSpan = LifeSpan.InstancePerLifetimeScope)
        {
            var x = _builder.Register(c =>
            {
                return constructionFunc();

            }).Named<InterfaceOfT>(serviceName);
            ApplyLifeTime(x, lifeSpan);
            return this;
        }


        public IIoCProvider RegisterBehavior<InterfaceOfT>(Func<IIoCServiceLocator, InterfaceOfT> constructionFunc, string serviceName, LifeSpan lifeSpan = LifeSpan.InstancePerLifetimeScope)
        {
            var x = _builder.Register(c =>
            {
                return constructionFunc(c.Resolve<IIoCServiceLocator>());

            }).Named<InterfaceOfT>(serviceName);
            ApplyLifeTime(x, lifeSpan);
            return this;

        }

        public IIoCProvider RegisterBehavior<InterfaceOfT>(Func<IIoCServiceLocator, InterfaceOfT> constructionFunc, LifeSpan lifeSpan = LifeSpan.InstancePerLifetimeScope)
        {
            var x = _builder.Register(c =>
            {
                return constructionFunc(c.Resolve<IIoCServiceLocator>());

            }).As<InterfaceOfT>();

            ApplyLifeTime(x, lifeSpan);
            return this;
        }

        /// <summary>
        /// Registers the with parameters.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parameters">The parameters.</param>
        /// <returns>IIoCProvider.</returns>
        public IIoCProvider RegisterWithParameters<T>(params IoC.Parameter[] parameters)
        {
            var i = 0;
            var autofacTypeParameters = parameters
                .Select(x => new PositionalParameter(i++, x.Value));

            _builder.RegisterType<T>().WithParameters(autofacTypeParameters);

            return this;
        }

        /// <summary>
        /// Gets the named.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serviceName">Name of the service.</param>
        /// <returns>T.</returns>
        public T GetNamed<T>(string serviceName)
        {
            return _container.ResolveNamed<T>(serviceName);
        }

        /// <summary>
        /// Gets the primary lifetime scope.
        /// </summary>
        /// <returns>ILifetimeScope.</returns>
        public ILifetimeScope GetPrimaryLifetimeScope()
        {
            return _container;
        }

        /// <summary>
        /// Creates the child lifetime scope.
        /// </summary>
        /// <returns>IIoCServiceLocator.</returns>
        public IIoCProvider CreateChildLifetimeScope()
        {
            return new AutofacIoCProvider(_container.BeginLifetimeScope());
        }

        public IIoCProvider CreateChildLifetimeScope(Action<IIoCProvider> containerBuilder)
        {
            return new AutofacIoCProvider(_container.BeginLifetimeScope(containerBuilder));
        }


        /// <summary>
        /// Creates the child lifetime scope.
        /// </summary>
        /// <param name="scopeName">Name of the scope.</param>
        /// <returns>IIoCServiceLocator.</returns>
        public IIoCProvider CreateChildLifetimeScope(string scopeName)
        {

            return new AutofacIoCProvider(_container.BeginLifetimeScope(scopeName));
        }

        /// <summary>
        /// Registers the modules.
        /// </summary>
        /// <returns>IIoCProvider.</returns>
        public IIoCProvider RegisterModules()
        {
            RegisterModules($@"{AppDomain.CurrentDomain.RelativeSearchPath}\autofac.json");
            return this;
        }

        public IIoCProvider RegisterModuleInstance(ModuleBase packageModule)
        {
            packageModule.GuardIsNotNull(nameof(instance));
            packageModule.Configure(this);
            return this;
        }

        /// <summary>
        /// Registers the modules.
        /// </summary>
        /// <param name="configFilePath">The configuration file path.</param>
        /// <returns>IIoCProvider.</returns>
        public IIoCProvider RegisterModules(string configFilePath)
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile(configFilePath)
                .Build();

            var configModule = new ConfigurationModule(configuration);
            _builder.RegisterModule(configModule);

            return this;
        }

        /// <summary>
        /// Registers the self.
        /// </summary>
        /// <returns>IIoCProvider.</returns>
        public IIoCProvider RegisterSelf()
        {
            _builder.RegisterInstance<IIoCProvider>(this)
                .AsImplementedInterfaces();

            _builder.RegisterType<AutofacServiceLocator>()
                .As<IIoCServiceLocator>()
                .InstancePerMatchingLifetimeScope("func-request")
                .InstancePerLifetimeScope()
                .AsImplementedInterfaces();
            return this;
        }


        /// <summary>
        /// Registers the owin middleware.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>IIoCProvider.</returns>
        public IIoCProvider RegisterOwinMiddleware<T>()
        {
            _builder.RegisterType<T>()
                .InstancePerRequest();
            return this;
        }

        public IIoCProvider RegisterMicrosoftDiServiceProvider(IServiceCollection services)
        {
            _builder.Populate(services ?? new ServiceCollection());
            return this;
        }


        /// <summary>
        /// Registers this instance.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="InterfaceOfT">The type of the interface of t.</typeparam>
        /// <returns>IIoCProvider.</returns>
        public IIoCProvider Register<T, InterfaceOfT>(LifeSpan lifeSpan = LifeSpan.InstancePerLifetimeScope)
        {
            var x = _builder.RegisterType<T>().As<InterfaceOfT>();
            ApplyLifeTime(x, lifeSpan);
            return this;
        }

        public IIoCProvider RegisterNamed<T, InterfaceOfT>(string serviceName, LifeSpan lifeSpan = LifeSpan.InstancePerLifetimeScope)
        {
            var x = _builder
                .RegisterType<T>()
                .Named<InterfaceOfT>(serviceName);
            ApplyLifeTime(x, lifeSpan);
            return this;
        }

        public IIoCProvider Register(Type type, Type interfaceOfT, LifeSpan lifeSpan = LifeSpan.InstancePerLifetimeScope)
        {
            var x = _builder.RegisterType(type).As(interfaceOfT);
            ApplyLifeTime(x, lifeSpan);
            return this;
        }

        public IIoCProvider RegisterNamed(string serviceName, Type type, Type interfaceOfT, LifeSpan lifeSpan = LifeSpan.InstancePerLifetimeScope)
        {
            var x = _builder.RegisterType(type).Named(serviceName, interfaceOfT);
            ApplyLifeTime(x, lifeSpan);
            return this;
        }

        private void ApplyLifeTime<T>(Autofac.Builder.IRegistrationBuilder<T, Autofac.Builder.IConcreteActivatorData, Autofac.Builder.SingleRegistrationStyle> x, LifeSpan lifeSpan)
        {
            switch (lifeSpan)
            {
                case LifeSpan.InstancePerLifetimeScope:
                    x.InstancePerLifetimeScope();
                    break;
                case LifeSpan.NewInstancePerRequest:
                    x.InstancePerRequest();
                    break;
                case LifeSpan.SingleInstance:
                    x.SingleInstance();
                    break;
                case LifeSpan.ExternallyOwned:
                    x.ExternallyOwned();
                    break;
            }
        }

        private void ApplyLifeTime(IRegistrationBuilder<object, ScanningActivatorData, DynamicRegistrationStyle> x, LifeSpan lifeSpan)
        {
            switch (lifeSpan)
            {
                case LifeSpan.InstancePerLifetimeScope:
                    x.InstancePerLifetimeScope();
                    break;
                case LifeSpan.NewInstancePerRequest:
                    x.InstancePerRequest();
                    break;
                case LifeSpan.SingleInstance:
                    x.SingleInstance();
                    break;
            }
        }



        /// <summary>
        /// Registers the singleton.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="InterfaceOfT">The type of the interface of t.</typeparam>
        /// <returns>IIoCProvider.</returns>
        public IIoCProvider RegisterSingleton<T, InterfaceOfT>()
        {
            _builder.RegisterType<T>().As<InterfaceOfT>().SingleInstance();
            return this;
        }

        /// <summary>
        /// Registers the instance.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance">The instance.</param>
        /// <returns>IIoCProvider.</returns>
        public IIoCProvider RegisterInstance<T>(T instance)
            where T : class
        {
            _builder.RegisterInstance(instance).SingleInstance();
            return this;
        }

        /// <summary>
        /// Registers the instance as.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="InterfaceOfT">The type of the interface of t.</typeparam>
        /// <param name="instance">The instance.</param>
        /// <returns>IIoCProvider.</returns>
        public IIoCProvider RegisterInstanceAs<T, InterfaceOfT>(T instance)
          where T : class
        {
            _builder.RegisterInstance(instance).As<InterfaceOfT>().SingleInstance();
            return this;
        }

        #region Trident Using Feature

        public IIoCProvider UsingTridentEntityComparer()
        {
            _builder.UsingTridentEntityComparer();
            return this;
        }

        public IIoCProvider UsingTridentFileStorage()
        {
            _builder.UsingTridentFileStorage();
            return this;
        }

        public IIoCProvider UsingTridentData()
        {
            _builder.UsingTridentData();
            return this;
        }

        public IIoCProvider UsingTridentMapperProfiles(params Assembly[] targetAssemblies)
        {
            _builder.UsingTridentMapperProfiles(targetAssemblies);
            return this;
        }

        public IIoCProvider UsingTridentTransactions()
        {
            _builder.UsingTridentTransactions();
            return this;
        }

        public IIoCProvider UsingTridentInMemberCachingManager()
        {
            _builder.UsingTridentInMemberCachingManager();
            return this;
        }

        public IIoCProvider UsingTridentValidationManagers(params Assembly[] targetAssemblies)
        {
            _builder.UsingTridentValidationManagers(targetAssemblies);
            return this;
        }

        public IIoCProvider UsingTridentValidationRules(params Assembly[] targetAssemblies)
        {
            _builder.UsingTridentValidationRules(targetAssemblies);
            return this;
        }

        public IIoCProvider UsingTridentWorkflowManagers(params Assembly[] targetAssemblies)
        {
            _builder.UsingTridentWorkflowManagers(targetAssemblies);
            return this;
        }

        public IIoCProvider UsingTridentWorkflowTasks(params Assembly[] targetAssemblies)
        {
            _builder.UsingTridentWorkflowTasks(targetAssemblies);
            return this;
        }

        public IIoCProvider UsingTridentProviders(params Assembly[] targetAssemblies)
        {
            _builder.UsingTridentProviders(targetAssemblies);
            return this;
        }


        public IIoCProvider UsingTridentManagers(params Assembly[] targetAssemblies)
        {
            _builder.UsingTridentManagers(targetAssemblies);
            return this;
        }

        public IIoCProvider UsingTridentRepositories(params Assembly[] targetAssemblies)
        {
            _builder.UsingTridentRepositories(targetAssemblies);
            return this;
        }

        /// <summary>
        /// Registers the data source dependencies.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="targetAssemblies">The target assemblies.</param>
        public IIoCProvider UsingTridentSearch(params Assembly[] targetAssemblies)
        {
            _builder.UsingTridentSearch(targetAssemblies);
            return this;
        }

        public IIoCProvider UsingTridentResolvers(params Assembly[] targetAssemblies)
        {
            _builder.UsingTridentResolvers(targetAssemblies);
            return this;
        }

        public IIoCProvider UsingTridentFactories(params Assembly[] targetAssemblies)
        {
            _builder.UsingTridentFactories(targetAssemblies);
            return this;
        }

        public IIoCProvider UsingTridentStrategy<T>(params Assembly[] targetAssemblies)
        {
            _builder.UsingTridentStrategy<T>(targetAssemblies);
            return this;
        }


        public IIoCProvider UsingTridentAppSettingsXmlManager()

        {
            _builder.UsingTridentAppSettingsXmlManager();
            return this;
        }

        public IIoCProvider UsingTridentAppSettingsJsonManager()

        {
            _builder.UsingTridentAppSettingsJsonManager();
            return this;
        }

        public IIoCProvider UsingTridentConnectionStringXmlManager()
        {
            _builder.UsingTridentConnectionStringXmlManager();
            return this;
        }

        public IIoCProvider UsingTridentConnectionStringJsonManager()
        {
            _builder.UsingTridentConnectionStringJsonManager();
            return this;
        }

        public IIoCProvider UsingTridentLoggerFactory(ILoggerFactory loggerFactory)
        {
            _builder.RegisterInstance(loggerFactory)
                .As<ILoggerFactory>();
            return this;
        }

        #endregion

        /// <summary>
        /// Builds this instance.
        /// </summary>
        public void Build()
        {
            _container = _builder.Build();
            var locator = _container.Resolve<IIoCServiceLocator>();
        }

        /// <summary>
        /// Verifies this instance.
        /// </summary>
        public virtual void Verify()
        {
            Verify(false, true);
        }

        /// <summary>
        /// Verifies the and throw.
        /// </summary>
        public virtual void VerifyAndThrow()
        {
            Verify(true, true);
        }

        /// <summary>
        /// Verifies the specified throw exceptions.
        /// </summary>
        /// <param name="throwExceptions">if set to <c>true</c> [throw exceptions].</param>
        /// <param name="logErrors">if set to <c>true</c> [log errors].</param>
        /// <exception cref="DependencyResolutionException">
        /// </exception>
        private void Verify(bool throwExceptions, bool logErrors)
        {

            var allRegistrations = _container.ComponentRegistry
                   .Registrations.Where(x => x.Services.Count() == 1)
                   .Select(x =>
                   {

                       var keyService = x.Services.OfType<KeyedService>().FirstOrDefault();
                       return new
                       {
                           RegistrationType = (keyService != null)
                               ? keyService.ServiceType
                               : x.Services.OfType<TypedService>().First().ServiceType,
                           NameKey = (keyService != null)
                               ? keyService.ServiceKey
                               : null,
                           InstanceType = x.Activator.LimitType
                       };
                   })
                   .ToList();

            foreach (var reg in allRegistrations)
            {
                using (var requestScope = _container.BeginLifetimeScope("AutofacWebRequest"))
                {
                    try
                    {
                        object actual = null;
                        if (reg.NameKey != null)
                        {
                            System.Diagnostics.Debug.WriteLine(string.Format("Named: {0} - {1} ", reg.NameKey, reg.RegistrationType.FullName));
                            actual = requestScope.ResolveKeyed(reg.NameKey, reg.RegistrationType);
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine(string.Format("Named: Default - {0}", reg.RegistrationType.FullName));

                            actual = requestScope.Resolve(reg.RegistrationType);
                        }

                        if (actual == null)
                            throw new DependencyResolutionException(
                                string.Format("Unable to resolve {0} with key {1} or one of its dependencies.", reg.RegistrationType.FullName, reg.NameKey));

                        actual = GetTargetIfProxyOrOriginal(actual) ?? actual;

                        if (!reg.RegistrationType.IsAssignableFrom(actual.GetType()))
                            throw new DependencyResolutionException(
                             string.Format("Resolved named registration with key {0} but type was unexpected. Unexpected type {1}, expected {2}.",
                                   reg.NameKey, actual.GetType().FullName, reg.InstanceType.FullName));

                        //log overwritten registrations
                        if (!reg.InstanceType.IsAssignableFrom(actual.GetType()))
                            System.Diagnostics.Debug.WriteLine(
                                string.Format("Resolved named registration with key {0} but type was unexpected. Unexpected type {1}, expected {2}.",
                                   reg.NameKey, actual.GetType().FullName, reg.InstanceType.FullName));

                    }
                    catch (Exception ex)
                    {
                        if (logErrors)
                        {
                            //this longing is only good in vs, we need a way to
                            // log in production but this is the container, and we can't
                            //rely on a ioc registered logger.
                            System.Diagnostics.Debug.WriteLine(ex);
                        }

                        if (throwExceptions) throw;
                    }
                }
            }
        }


        /// <summary>
        /// Gets the target if proxy.
        /// NOTE: the easy cast was not used due to a hiesin bug in CastleProxy
        /// the cast or "as" statement only works in the quick watch when it is not
        /// code into the test. however using reflection to call the DynProxyGetTarget method
        /// as in this implementation works.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>System.Object.</returns>
        private object GetTargetIfProxyOrOriginal(object obj)
        {
            if (null != obj.GetType().GetInterface("Castle.DynamicProxy.IProxyTargetAccessor"))
            {
                var proxyInterface = obj.GetType().GetInterface("Castle.DynamicProxy.IProxyTargetAccessor");
                var getMethod = proxyInterface.GetMethod("DynProxyGetTarget");
                return getMethod.Invoke(obj, null);
            }

            return obj;
        }


        /// <summary>
        /// Gets or sets the container.
        /// </summary>
        /// <value>The container.</value>
        protected ILifetimeScope Container
        {
            get
            {
                return _container;
            }
            set
            {
                _container = value;
            }
        }

        public int Count => throw new NotImplementedException();

        public bool IsReadOnly => throw new NotImplementedException();

        public ServiceDescriptor this[int index] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                _disposed = true;
                if (disposing)
                {
                    using (_container) { }                   
                }
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Performs a dispose operation asynchronously.
        /// </summary>
        [SuppressMessage(
            "Usage",
            "CA1816:Dispose methods should call SuppressFinalize",
            Justification = "DisposeAsync should also call SuppressFinalize (see various .NET internal implementations).")]
        public async ValueTask DisposeAsync()
        {
            if (!_disposed)
            {
                _disposed = true;
                await _container.DisposeAsync().ConfigureAwait(false);
                GC.SuppressFinalize(this);
            }
        }

        public void Populate(IServiceCollection service)
        {
            _builder.Populate(service);
        }

        public IIoCProvider RegisterModules(Type[] moduleTypes)
        {
            foreach (var item in moduleTypes)
            {
                var instance = (IModule)Activator.CreateInstance(item);
                instance.GuardIsNotNull(nameof(instance));
                instance.Configure(this);
            }
            return this;
        }


    }
}
