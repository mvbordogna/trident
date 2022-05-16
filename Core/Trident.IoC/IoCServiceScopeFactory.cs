using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace Trident.IoC
{
    /// <summary>
    /// Autofac implementation of the ASP.NET Core <see cref="IServiceScopeFactory"/>.
    /// </summary>
    /// <seealso cref="Microsoft.Extensions.DependencyInjection.IServiceScopeFactory" />
    [SuppressMessage("Microsoft.ApiDesignGuidelines", "CA2213", Justification = "The creator of the root service lifetime scope is responsible for disposal.")]
    public class IoCServiceScopeFactory : IServiceScopeFactory
    {
        private readonly IIoCProvider _lifetimeScope;

        /// <summary>
        /// Initializes a new instance of the <see cref="IoCServiceScopeFactory"/> class.
        /// </summary>
        /// <param name="lifetimeScope">The lifetime scope.</param>
        public IoCServiceScopeFactory(IIoCProvider lifetimeScope)
        {
            _lifetimeScope = lifetimeScope;
        }

        /// <summary>
        /// Creates an <see cref="IServiceScope" /> which contains an
        /// <see cref="System.IServiceProvider" /> used to resolve dependencies within
        /// the scope.
        /// </summary>
        /// <returns>
        /// An <see cref="IServiceScope" /> controlling the lifetime of the scope. Once
        /// this is disposed, any scoped services that have been resolved
        /// from the <see cref="IServiceScope.ServiceProvider" />
        /// will also be disposed.
        /// </returns>
        public IServiceScope CreateScope()
        {
            return new IoCServiceScope(_lifetimeScope.CreateChildLifetimeScope());
        }
    }
}
