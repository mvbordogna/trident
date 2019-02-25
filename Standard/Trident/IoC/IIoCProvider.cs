﻿using System;
using System.Collections.Generic;

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
        IIoCProvider Register<T, InterfaceOfT>();

        /// <summary>
        /// Registers the behavior.
        /// </summary>
        /// <typeparam name="InterfaceOfT">The type of the interface of t.</typeparam>
        /// <param name="constructionFunc">The construction function.</param>
        /// <returns>IIoCProvider.</returns>
        IIoCProvider RegisterBehavior<InterfaceOfT>(Func<InterfaceOfT> constructionFunc);

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
    }
}