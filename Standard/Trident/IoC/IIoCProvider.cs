using System;
using System.Collections.Generic;
using System.Reflection;

namespace Trident.IoC
{
    /// <summary>
    /// Interface IIoCProvider
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public interface IIoCProvider : IDisposable
    {
        /// <summary>
        /// Gets this instance.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>T.</returns>
        T Get<T>();

        /// <summary>
        /// Gets the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>System.Object.</returns>
        object Get(Type type);

        /// <summary>
        /// Gets the specified parameters.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parameters">The parameters.</param>
        /// <returns>T.</returns>
        T Get<T>(params Parameter[] parameters);
        IIoCProvider RegisterBehavior<T>(Func<T> p, string name, LifeSpan lifeSpan = LifeSpan.InstancePerLifetimeScope);

        /// <summary>
        /// Resolves all keyed.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>IEnumerable&lt;T&gt;.</returns>
        IEnumerable<T> ResolveAllKeyed<T>();

        /// <summary>
        /// Resolves all typed.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>IEnumerable&lt;T&gt;.</returns>
        IEnumerable<T> ResolveAllTyped<T>();

        /// <summary>
        /// Gets the named.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serviceName">Name of the service.</param>
        /// <returns>T.</returns>
        T GetNamed<T>(string serviceName);

        /// <summary>
        /// Registers the modules.
        /// </summary>
        /// <returns>IIoCProvider.</returns>
        IIoCProvider RegisterModules();

        /// <summary>
        /// Registers the modules.
        /// </summary>
        /// <param name="configFilePath">The configuration file path.</param>
        /// <returns>IIoCProvider.</returns>
        IIoCProvider RegisterModules(string configFilePath);

        /// <summary>
        /// Registers the module.
        /// </summary>
        /// <param name="moduleType">Type of the module.</param>
        /// <returns>IIoCProvider.</returns>
        IIoCProvider RegisterModule(Type moduleType);

        /// <summary>
        /// Registers the self.
        /// </summary>
        /// <returns>IIoCProvider.</returns>
        IIoCProvider RegisterSelf();

        /// <summary>
        /// Registers this instance.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="InterfaceOfT">The type of the interface of t.</typeparam>
        /// <returns>IIoCProvider.</returns>
        IIoCProvider Register<T, InterfaceOfT>(LifeSpan lifeSpan = LifeSpan.InstancePerLifetimeScope);

        IIoCProvider RegisterNamed<T, InterfaceOfT>(string serviceName, LifeSpan lifeSpan = LifeSpan.InstancePerLifetimeScope);

        IIoCProvider RegisterAll<T>(Assembly[] targetAssemblies, LifeSpan lifeSpan = LifeSpan.InstancePerLifetimeScope);

        IIoCProvider Register(Type type, Type interfaceOfT, LifeSpan lifeSpan = LifeSpan.InstancePerLifetimeScope);

        IIoCProvider RegisterNamed(string serviceName, Type type, Type interfaceOfT, LifeSpan lifeSpan = LifeSpan.InstancePerLifetimeScope);


        /// <summary>
        /// Registers the behavior.
        /// </summary>
        /// <typeparam name="InterfaceOfT">The type of the interface of t.</typeparam>
        /// <param name="constructionFunc">The construction function.</param>
        /// <returns>IIoCProvider.</returns>
        IIoCProvider RegisterBehavior<InterfaceOfT>(Func<InterfaceOfT> constructionFunc, LifeSpan lifeSpan = LifeSpan.InstancePerLifetimeScope);

        /// <summary>
        /// Registers the singleton.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="InterfaceOfT">The type of the interface of t.</typeparam>
        /// <returns>IIoCProvider.</returns>
        IIoCProvider RegisterSingleton<T, InterfaceOfT>();

        /// <summary>
        /// Registers the instance.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance">The instance.</param>
        /// <returns>IIoCProvider.</returns>
        IIoCProvider RegisterInstance<T>(T instance)
            where T : class;

        /// <summary>
        /// Registers the instance as.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="InterfaceOfT">The type of the interface of t.</typeparam>
        /// <param name="instance">The instance.</param>
        /// <returns>IIoCProvider.</returns>
        IIoCProvider RegisterInstanceAs<T, InterfaceOfT>(T instance)
         where T : class;
        /// <summary>
        /// Verifies the and throw.
        /// </summary>
        void VerifyAndThrow();
        /// <summary>
        /// Builds this instance.
        /// </summary>
        void Build();

        /// <summary>
        /// Registers the with parameters.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parameters">The parameters.</param>
        /// <returns>IIoCProvider.</returns>
        IIoCProvider RegisterWithParameters<T>(params Parameter[] parameters);

        /// <summary>
        /// Registers the owin middleware.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>IIoCProvider.</returns>
        IIoCProvider RegisterOwinMiddleware<T>();


        #region Trident Using Feature Methods
        IIoCProvider UsingTridentFileStorage();
        IIoCProvider UsingTridentData();
        IIoCProvider UsingTridentMapperProfiles(params Assembly[] targetAssemblies);
        IIoCProvider UsingTridentTransactions();
        IIoCProvider UsingTridentValidationManagers(params Assembly[] targetAssemblies);
        IIoCProvider UsingTridentValidationRules(params Assembly[] targetAssemblies);
        IIoCProvider UsingTridentWorkflowManagers(params Assembly[] targetAssemblies);
        IIoCProvider UsingTridentWorkflowTasks(params Assembly[] targetAssemblies);
        IIoCProvider UsingTridentProviders(params Assembly[] targetAssemblies);
        IIoCProvider UsingTridentManagers(params Assembly[] targetAssemblies);
        IIoCProvider UsingTridentRepositories(params Assembly[] targetAssemblies);
        IIoCProvider UsingTridentSearch(params Assembly[] targetAssemblies);
        IIoCProvider UsingTridentResolvers(params Assembly[] targetAssemblies);
        IIoCProvider UsingTridentFactories(params Assembly[] targetAssemblies);
        IIoCProvider UsingTridentStrategy<T>(params Assembly[] targetAssemblies);
        IIoCProvider UsingTridentAppSettingsXmlManager();
        IIoCProvider UsingTridentAppSettingsJsonManager();
        IIoCProvider UsingTridentConnectionStringXmlManager();
        IIoCProvider UsingTridentConnectionStringJsonManager();

        #endregion


    }
}
