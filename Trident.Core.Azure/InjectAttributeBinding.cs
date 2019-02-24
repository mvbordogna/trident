using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Azure.WebJobs.Host.Protocols;
using Trident.Core.IoC;

namespace Trident.Core.Azure
{
    /// <summary>
    /// Class InjectAttributeBinding.
    /// Implements the <see cref="Microsoft.Azure.WebJobs.Host.Bindings.IBinding" />
    /// </summary>
    /// <seealso cref="Microsoft.Azure.WebJobs.Host.Bindings.IBinding" />
    public class InjectAttributeBinding : IBinding
    {
        /// <summary>
        /// The parameter information
        /// </summary>
        private readonly ParameterInfo _parameterInfo;
        /// <summary>
        /// The locator
        /// </summary>
        private readonly IIoCServiceLocator _locator;

        /// <summary>
        /// Initializes a new instance of the <see cref="InjectAttributeBinding"/> class.
        /// </summary>
        /// <param name="parameterInfo">The parameter information.</param>
        /// <param name="locator">The locator.</param>
        public InjectAttributeBinding(ParameterInfo parameterInfo, IIoCServiceLocator locator)
        {
            _parameterInfo = parameterInfo;
            _locator = locator;
        }

        /// <summary>
        /// Perform a bind to the specified value.
        /// </summary>
        /// <param name="value">The value to bind to.</param>
        /// <param name="context">The binding context.</param>
        /// <returns>A task that returns the <see cref="T:Microsoft.Azure.WebJobs.Host.Bindings.IValueProvider" /> for the binding.</returns>
        public Task<IValueProvider> BindAsync(object value, ValueBindingContext context)
        {       
            return Task.FromResult<IValueProvider>(new InjectAttributeValueProvider(_parameterInfo, _locator));
        }

        /// <summary>
        /// Perform a bind using the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>A task that returns the <see cref="T:Microsoft.Azure.WebJobs.Host.Bindings.IValueProvider" /> for the binding.</returns>
        public Task<IValueProvider> BindAsync(BindingContext context)
        {
            return Task.FromResult<IValueProvider>(new InjectAttributeValueProvider(_parameterInfo, _locator));
        }

        /// <summary>
        /// Get a description of the binding.
        /// </summary>
        /// <returns>ParameterDescriptor.</returns>
        public ParameterDescriptor ToParameterDescriptor()
        {
            return new ParameterDescriptor
            {
                Name = _parameterInfo.Name,
                DisplayHints = new ParameterDisplayHints
                {
                    Description = "Inject services",
                    DefaultValue = "Inject services",
                    Prompt = "Inject services"
                }
            };
        }

        /// <summary>
        /// Gets a value indicating whether the binding was sourced from a parameter attribute.
        /// </summary>
        /// <value><c>true</c> if [from attribute]; otherwise, <c>false</c>.</value>
        public bool FromAttribute => true;
    }
}