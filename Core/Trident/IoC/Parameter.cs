using System;

namespace Trident.IoC
{
    /// <summary>
    /// Class Parameter.
    /// </summary>
    public class Parameter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Parameter"/> class.
        /// </summary>
        /// <param name="parameterType">Type of the parameter.</param>
        /// <param name="value">The value.</param>
        public Parameter(Type parameterType, object value)
        {
            this.ParameterType = parameterType;
            this.Value = value;
        }
        /// <summary>
        /// Gets the type of the parameter.
        /// </summary>
        /// <value>The type of the parameter.</value>
        public Type ParameterType { get; private set; }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>The value.</value>
        public object Value { get; private set; }
    }
}
