using Autofac;
using Autofac.Configuration;
using Autofac.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using Trident.Extensions;
using Microsoft.Extensions.Configuration;
using Trident.IoC;

namespace Trident.Autofac
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
        private IContainer _container;
        /// <summary>
        /// The is initializing
        /// </summary>
        private bool isInitializing = true;
        /// <summary>
        /// The instance
        /// </summary>
        private static IIoCProvider instance = null;
        /// <summary>
        /// The pad lock
        /// </summary>
        private static readonly object pad_lock = new object();

        /// <summary>
        /// Initializes a new instance of the <see cref="AutofacIoCProvider"/> class.
        /// </summary>
        /// <param name="builder">The builder.</param>
        protected AutofacIoCProvider(ContainerBuilder builder)
        {
            _builder = builder;
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


        /// <summary>
        /// Gets this instance.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>T.</returns>
        public T Get<T>()
        {
            return _container.Resolve<T>();
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
            where T : Module, new()
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

            var module = System.Activator.CreateInstance(moduleType) as IModule;

            if (module != null)
                _builder.RegisterModule(module);
            else throw new ArgumentOutOfRangeException($"The Type provider {moduleType.FullName} must implement {typeof(IModule).FullName}");

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

        /// <summary>
        /// Registers the behavior.
        /// </summary>
        /// <typeparam name="InterfaceOfT">The type of the interface of t.</typeparam>
        /// <param name="constructionFunc">The construction function.</param>
        /// <returns>IIoCProvider.</returns>
        public IIoCProvider RegisterBehavior<InterfaceOfT>(Func<InterfaceOfT> constructionFunc)
        {
            _builder.Register(c =>
            {
                return constructionFunc();

            }).As<InterfaceOfT>();

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
        /// Initializes the specified new builder.
        /// </summary>
        /// <param name="newBuilder">The new builder.</param>
        public void Initialize(ContainerBuilder newBuilder)
        {
            newBuilder.Update(_container);
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
        public IIoCServiceLocator CreateChildLifetimeScope()
        {
            return new AutofacServiceLocator(_container.BeginLifetimeScope());
        }

        /// <summary>
        /// Creates the child lifetime scope.
        /// </summary>
        /// <param name="scopeName">Name of the scope.</param>
        /// <returns>IIoCServiceLocator.</returns>
        public IIoCServiceLocator CreateChildLifetimeScope(string scopeName)
        {
          
            return new AutofacServiceLocator(_container.BeginLifetimeScope(scopeName));
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
            _builder.RegisterInstance<IIoCProvider>(this);
            _builder.RegisterType<AutofacServiceLocator>()
                .As<IIoCServiceLocator>().InstancePerLifetimeScope();
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


        /// <summary>
        /// Registers this instance.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="InterfaceOfT">The type of the interface of t.</typeparam>
        /// <returns>IIoCProvider.</returns>
        public IIoCProvider Register<T, InterfaceOfT>()
        {
            _builder.RegisterType<T>().As<InterfaceOfT>();
            return this;
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

        /// <summary>
        /// Builds this instance.
        /// </summary>
        public void Build()
        {
            _container = _builder.Build();
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
        protected IContainer Container
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


        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            using (_container) { }
        }

      
    }
}
