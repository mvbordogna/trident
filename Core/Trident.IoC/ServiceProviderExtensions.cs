using System;
using System.Globalization;

namespace Trident.IoC
{
    /// <summary>
    /// Extension methods for use with the <see cref="IoCServiceProvider"/>.
    /// </summary>
    public static class ServiceProviderExtensions
    {
        /// <summary>
        /// Tries to cast the instance of <see cref="ILifetimeScope"/> from <see cref="IoCServiceProvider"/> when possible.
        /// </summary>
        /// <param name="serviceProvider">The instance of <see cref="IServiceProvider"/>.</param>
        /// <exception cref="InvalidOperationException">Throws an <see cref="InvalidOperationException"/> when instance of <see cref="IServiceProvider"/> can't be assigned to <see cref="IoCServiceProvider"/>.</exception>
        /// <returns>Returns the instance of <see cref="ILifetimeScope"/> exposed by <see cref="IoCServiceProvider"/>.</returns>
        public static IIoCProvider GetAutofacRoot(this IServiceProvider serviceProvider)
        {
            if (!(serviceProvider is IoCServiceProvider autofacServiceProvider))
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, ServiceProviderExtensionsResources.WrongProviderType, serviceProvider?.GetType()));

            return autofacServiceProvider.LifetimeScope;
        }
    }
}
