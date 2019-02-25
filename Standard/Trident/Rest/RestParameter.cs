namespace Trident.Rest
{
    /// <summary>
    /// REST Parameters
    /// </summary>
    public class RestParameter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RestParameter" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <param name="type">The type.</param>
        public RestParameter(string name, object value, RestParameterType type)
        {
            Name = name;
            Value = value;
            Type = type;
        }

        /// <summary>
        /// Name of the parameter
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }

        /// <summary>
        /// Value of the parameter
        /// </summary>
        /// <value>The value.</value>
        public object Value { get; set; }

        /// <summary>
        /// Type of the parameter
        /// </summary>
        /// <value>The type.</value>
        public RestParameterType Type { get; set; }
    }
}