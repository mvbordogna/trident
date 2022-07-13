using Microsoft.Extensions.DependencyInjection;
using System;

namespace Trident.IoC
{
    /// <summary>
    /// A factory for creating a <see cref="IServiceProvider"/> that wraps a child <see cref="ILifetimeScope"/> created from an existing parent <see cref="ILifetimeScope"/>.
    /// </summary>
    public class IoCChildLifetimeScopeServiceProviderFactory : IServiceProviderFactory<IoCChildLifetimeScopeConfigurationAdapter>
    {
        private readonly Action<IIoCProvider> _containerConfigurationAction;
        private readonly IIoCProvider _rootLifetimeScope;
        private static readonly Action<IIoCProvider> FallbackConfigurationAction = builder => { };

        /// <summary>
        /// Initializes a new instance of the <see cref="IoCChildLifetimeScopeServiceProviderFactory"/> class.
        /// </summary>
        /// <param name="rootLifetimeScopeAccessor">A function to retrieve the root <see cref="ILifetimeScope"/> instance.</param>
        /// <param name="configurationAction">Action on a <see cref="ContainerBuilder"/> that adds component registrations to the container.</param>
        public IoCChildLifetimeScopeServiceProviderFactory(Func<IIoCProvider> rootLifetimeScopeAccessor, Action<IIoCProvider> configurationAction = null)
        {
            if (rootLifetimeScopeAccessor == null) throw new ArgumentNullException(nameof(rootLifetimeScopeAccessor));

            _rootLifetimeScope = rootLifetimeScopeAccessor();
            _containerConfigurationAction = configurationAction ?? FallbackConfigurationAction;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IoCChildLifetimeScopeServiceProviderFactory"/> class.
        /// </summary>
        /// <param name="rootLifetimeScope">The root <see cref="ILifetimeScope"/> instance.</param>
        /// <param name="configurationAction">Action on a <see cref="ContainerBuilder"/> that adds component registrations to the container.</param>
        public IoCChildLifetimeScopeServiceProviderFactory(IIoCProvider rootLifetimeScope, Action<IIoCProvider> configurationAction = null)
        {
            _rootLifetimeScope = rootLifetimeScope ?? throw new ArgumentNullException(nameof(rootLifetimeScope));
            _containerConfigurationAction = configurationAction ?? FallbackConfigurationAction;
        }

        /// <summary>
        /// Creates a container builder from an <see cref="IServiceCollection" />.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <returns>A container builder that can be used to create an <see cref="IServiceProvider" />.</returns>
        public IoCChildLifetimeScopeConfigurationAdapter CreateBuilder(IServiceCollection services)
        {
            var actions = new IoCChildLifetimeScopeConfigurationAdapter();

            actions.Add(builder => builder.Populate(services));
            actions.Add(builder => _containerConfigurationAction(builder));

            return actions;
        }

        /// <summary>
        /// Creates an <see cref="IServiceProvider" /> from the container builder.
        /// </summary>
        /// <param name="containerBuilder">The adapter holding configuration applied to <see cref="ContainerBuilder"/> creating the <see cref="IServiceProvider"/>.</param>
        /// <returns>An <see cref="IServiceProvider" />.</returns>
        public IServiceProvider CreateServiceProvider(IoCChildLifetimeScopeConfigurationAdapter containerBuilder)
        {
            if (containerBuilder == null) throw new ArgumentNullException(nameof(containerBuilder));

            var scope = _rootLifetimeScope.CreateChildLifetimeScope(scopeBuilder =>
            {
                foreach (var action in containerBuilder.ConfigurationActions)
                {
                    action(scopeBuilder);
                }
            });

            return new IoCServiceProvider(scope);
        }
    }
}
