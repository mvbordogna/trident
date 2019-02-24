using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Trident.IoC;

namespace Trident.Azure
{
    /// <summary>
    /// Class InjectAttributeValueProvider.
    /// Implements the <see cref="Microsoft.Azure.WebJobs.Host.Bindings.IValueProvider" />
    /// </summary>
    /// <seealso cref="Microsoft.Azure.WebJobs.Host.Bindings.IValueProvider" />
    internal class InjectAttributeValueProvider : IValueProvider
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
        /// Initializes a new instance of the <see cref="InjectAttributeValueProvider" /> class.
        /// </summary>
        /// <param name="parameterInfo">The parameter information.</param>
        /// <param name="locator">The locator.</param>
        public InjectAttributeValueProvider(ParameterInfo parameterInfo, IIoCServiceLocator locator)
        {
            _parameterInfo = parameterInfo;
            _locator = locator;
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <returns>A task that returns the value.</returns>
        public Task<object> GetValueAsync()
        {
            var injectAttribute = _parameterInfo.GetCustomAttribute<InjectAttribute>();
            if (!injectAttribute.HasName)
            {
                return Task.FromResult(_locator.Get(Type));
            }
            return Task.FromResult(_locator.GetNamed(Type, injectAttribute.Name));

        }

        /// <summary>
        /// Returns a string representation of the value.
        /// </summary>
        /// <returns>The string representation of the value.</returns>
        public string ToInvokeString()
        {
            return Type.ToString();
        }

        /// <summary>
        /// Gets the <see cref="P:Microsoft.Azure.WebJobs.Host.Bindings.IValueProvider.Type" /> of the value.
        /// </summary>
        /// <value>The type.</value>
        public Type Type => _parameterInfo.ParameterType;
    }
}