﻿using Autofac;
using Autofac.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

namespace Trident.IoC
{
    /// <summary>
    /// Class AutofacServiceLocator.
    /// Implements the <see cref="Trident.IIoCServiceLocator" />
    /// </summary>
    /// <seealso cref="Trident.IIoCServiceLocator" />
    public class AutofacServiceLocator : IIoCServiceLocator
    {

        private readonly Guid InstanceId = Guid.NewGuid();

        /// <summary>
        /// The container scope
        /// </summary>
        private ILifetimeScope _containerScope;
        /// <summary>
        /// Gets a value indicating whether this instance is a child scoped.
        /// </summary>
        /// <value><c>true</c> if this instance is child scoped; otherwise, <c>false</c>.</value>
        private bool IsChildScoped { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AutofacServiceLocator"/> class.
        /// </summary>
        /// <param name="containerScope">The container scope.</param>
        /// <param name="isChildScoped">if set to <c>true</c> [is child scoped].</param>
        public AutofacServiceLocator(ILifetimeScope containerScope, bool isChildScoped = false)
        {
            _containerScope = containerScope;
            IsChildScoped = isChildScoped;
        }

        /// <summary>
        /// Gets the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>System.Object.</returns>
        public object Get(Type type)
        {
            return _containerScope.Resolve(type);
        }

        /// <summary>
        /// Gets this instance.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>T.</returns>
        public T Get<T>()
        {
            return _containerScope.Resolve<T>();
        }

        public bool TryGet<T>(out T service)
        {
            try
            {
                service = _containerScope.Resolve<T>();
                return true;
            }
            catch (Exception)
            {
                service = default(T);
                return false;
            }
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

            return _containerScope.Resolve<T>(autofacTypeParameters);
        }

        /// <summary>
        /// Resolves all keyed.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>IEnumerable&lt;T&gt;.</returns>
        public IEnumerable<T> ResolveAllKeyed<T>()
        {
            return _containerScope.ComponentRegistry.Registrations
              .SelectMany(x => x.Services)
              .OfType<KeyedService>()
              .Where(x => x.ServiceType == typeof(T))
              .Select(t => _containerScope.ResolveKeyed<T>(t.ServiceKey));
        }

        /// <summary>
        /// Resolves all typed.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>IEnumerable&lt;T&gt;.</returns>
        public IEnumerable<T> ResolveAllTyped<T>()
        {
            return _containerScope.Resolve<IEnumerable<T>>();
        }


        public IEnumerable ResolveAllTyped(Type ofType)
        {
            var genType = typeof(IEnumerable<>).MakeGenericType(ofType);
            return _containerScope.Resolve(genType) as IEnumerable;
        }

        /// <summary>
        /// Gets the named.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serviceName">Name of the service.</param>
        /// <returns>T.</returns>
        public T GetNamed<T>(string serviceName)
        {
            return _containerScope.ResolveNamed<T>(serviceName);
        }

        /// <summary>
        /// Gets the named.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serviceName">Name of the service.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>T.</returns>
        public T GetNamed<T>(string serviceName, params IoC.Parameter[] parameters)
        {
            var autofacTypeParameters = parameters
              .Select(x => new TypedParameter(x.ParameterType, x.Value));

            return _containerScope.ResolveNamed<T>(serviceName, autofacTypeParameters);
        }

        /// <summary>
        /// Gets the named.
        /// </summary>
        /// <param name="serviceRegistrationType">Type of the service registration.</param>
        /// <param name="serviceName">Name of the service.</param>
        /// <returns>System.Object.</returns>
        public object GetNamed(Type serviceRegistrationType, string serviceName)
        {
            return _containerScope.ResolveNamed(serviceName, serviceRegistrationType);
        }

        /// <summary>
        /// Creates the child lifetime scope.
        /// </summary>
        /// <returns>IIoCServiceLocator.</returns>
        public IIoCServiceLocator CreateChildLifetimeScope()
        {
            return new AutofacServiceLocator(_containerScope.BeginLifetimeScope(), true);
        }

        /// <summary>
        /// Creates the child lifetime scope.
        /// </summary>
        /// <param name="scopeName">Name of the scope.</param>
        /// <returns>IIoCServiceLocator.</returns>
        public IIoCServiceLocator CreateChildLifetimeScope(string scopeName)
        { 
            var scope = _containerScope.BeginLifetimeScope(scopeName);
            var locator = (AutofacServiceLocator)scope.Resolve<IIoCServiceLocator>();
            return locator.SetContainer(scope, true);
            //return new AutofacServiceLocator(scope, true);
        }

        private IIoCServiceLocator SetContainer(ILifetimeScope scope, bool childScoped)
        {
            this._containerScope = scope;
            this.IsChildScoped = childScoped;
            return this;
        }

        /// <summary>
        /// Gets the name of the scope.
        /// </summary>
        /// <value>The name of the scope.</value>
        public string ScopeName
        {
            get
            {
                return _containerScope?.Tag?.ToString();
            }
        }


        /// <summary>
        /// Only Disponse this object if it is a child scope service locator
        /// </summary>
        public void Dispose()
        {
           if(_containerScope != null)
            {
                _containerScope.Dispose();
            }
        }

        public object GetService(Type serviceType)
        {
            return this.Get(serviceType);
        }
    }
}
