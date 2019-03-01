using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Trident.IoC;

namespace Trident.Azure
{
    /// <summary>
    /// Class InjectAttributeBindingProvider.
    /// Implements the <see cref="Microsoft.Azure.WebJobs.Host.Bindings.IBindingProvider" />
    /// </summary>
    /// <seealso cref="Microsoft.Azure.WebJobs.Host.Bindings.IBindingProvider" />
    public class InjectAttributeBindingProvider : IBindingProvider
    {
        /// <summary>
        /// The ioc initializer
        /// </summary>
        private readonly IIoCInitializer _iocInitializer;

        /// <summary>
        /// Initializes a new instance of the <see cref="InjectAttributeBindingProvider"/> class.
        /// </summary>
        public InjectAttributeBindingProvider()
        {
            _iocInitializer = new AutofacIoCInitializer();
        }

        /// <summary>
        /// Try to create a binding using the specified context.
        /// </summary>
        /// <param name="context">The binding context.</param>
        /// <returns>A task that returns the binding on completion.</returns>
        /// <exception cref="System.ArgumentNullException">context</exception>
        public Task<IBinding> TryCreateAsync(BindingProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var parameterInfo = context.Parameter;
            var injectAttribute = parameterInfo.GetCustomAttribute<InjectAttribute>();
            if (injectAttribute == null)
            {
                return Task.FromResult<IBinding>(null);
            }

            var locator = _iocInitializer.GetServiceLocator();           
            return Task.FromResult<IBinding>(new InjectAttributeBinding(parameterInfo, locator));
        }
    }
}