using Microsoft.Extensions.DependencyInjection;

namespace Trident.IoC
{
    /// <summary>
    /// A factory for creating a <see cref="ContainerBuilder"/> and an <see cref="IServiceProvider" />.
    /// </summary>
    public class IoCServiceProviderFactory<T> : IServiceProviderFactory<IIoCProvider>
        where T : class, IIoCProvider, new()
    {
        private readonly Action<IIoCProvider> _configurationAction;
        private readonly ContainerBuildOptions _containerBuildOptions = ContainerBuildOptions.None;

        /// <summary>
        /// Initializes a new instance of the <see cref="IoCServiceProviderFactory"/> class.
        /// </summary>
        /// <param name="containerBuildOptions">The container options to use when building the container.</param>
        /// <param name="configurationAction">Action on a <see cref="ContainerBuilder"/> that adds component registrations to the container.</param>
        public IoCServiceProviderFactory(
            ContainerBuildOptions containerBuildOptions,
            Action<IIoCProvider> configurationAction = null)
            : this(configurationAction) =>
            _containerBuildOptions = containerBuildOptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="IoCServiceProviderFactory"/> class.
        /// </summary>
        /// <param name="configurationAction">Action on a <see cref="ContainerBuilder"/> that adds component registrations to the container..</param>
        public IoCServiceProviderFactory(Action<IIoCProvider> configurationAction = null) =>
            _configurationAction = configurationAction ?? (builder => { });

        /// <summary>
        /// Creates a container builder from an <see cref="IServiceCollection" />.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <returns>A container builder that can be used to create an <see cref="IServiceProvider" />.</returns>
        public IIoCProvider CreateBuilder(IServiceCollection services)
        {
            var builder = new T();

            builder.Populate(services);

            _configurationAction(builder);

            return builder;
        }

        /// <summary>
        /// Creates an <see cref="IServiceProvider" /> from the container builder.
        /// </summary>
        /// <param name="containerBuilder">The container builder.</param>
        /// <returns>An <see cref="IServiceProvider" />.</returns>
        public IServiceProvider CreateServiceProvider(IIoCProvider containerBuilder)
        {
            if (containerBuilder == null) throw new ArgumentNullException(nameof(containerBuilder));

            containerBuilder.Build();

            return new IoCServiceProvider(containerBuilder);
        }
    }
}
