using Microsoft.Azure.WebJobs.Host.Config;

namespace Trident.Core.Azure
{
    /// <summary>
    /// Class InjectAttributeExtensionConfigProvider.
    /// Implements the <see cref="Microsoft.Azure.WebJobs.Host.Config.IExtensionConfigProvider" />
    /// </summary>
    /// <seealso cref="Microsoft.Azure.WebJobs.Host.Config.IExtensionConfigProvider" />
    public class InjectAttributeExtensionConfigProvider : IExtensionConfigProvider
    {
        /// <summary>
        /// The binding provider
        /// </summary>
        private readonly InjectAttributeBindingProvider _bindingProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="InjectAttributeExtensionConfigProvider"/> class.
        /// </summary>
        public InjectAttributeExtensionConfigProvider()
        {
            _bindingProvider = new InjectAttributeBindingProvider();
        }

        /// <summary>
        /// Initializes the extension. Initialization should register any extension bindings
        /// with the <see cref="T:Microsoft.Azure.WebJobs.Host.IExtensionRegistry" /> instance, which can be obtained from the
        /// <see cref="T:Microsoft.Azure.WebJobs.JobHostConfiguration" /> which is an <see cref="T:System.IServiceProvider" />.
        /// </summary>
        /// <param name="context">The <see cref="T:Microsoft.Azure.WebJobs.Host.Config.ExtensionConfigContext" /></param>
        public void Initialize(ExtensionConfigContext context)
        {
            context.AddBindingRule<InjectAttribute>().Bind(_bindingProvider);
        }
    }
}