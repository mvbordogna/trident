using System;
using Trident.EFCore;
using Trident.Extensions;

namespace Trident.Data
{
    /// <summary>
    /// Attribute that defines that an entity uses a sequence for it's Id property.
    /// </summary>
    /// <seealso cref="System.Attribute" />
    public class SequenceAttribute : Attribute
    {
        public SequenceAttribute(string name)
        {
            name.GuardIsNotNull(nameof(name));
            this.Name = name;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SequenceAttribute"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="schema">The schema.</param>
        public SequenceAttribute(string name, string schema = "dbo", DataAccessProviderTypes dataAccessProvider = DataAccessProviderTypes.SqlServer)
        {
            name.GuardIsNotNull(nameof(name));
            this.Name = name;
            this.Schema = schema;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SequenceAttribute"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="schema">The schema.</param>
        /// <param name="manualSequence">Manual sequence, if <see langword="true"/> sequence must be set manually. Defaults to <see langword="false"/>.</param>
        /// <param name="targetProperty">The target property that will have the sequence. Defaults to "Id"</param>
        public SequenceAttribute(string name, string schema = "dbo", bool manualSequence = false, string targetProperty = "Id")
        {
            name.GuardIsNotNull(nameof(name));
            this.Name = name;
            this.Schema = schema;
            this.ManualSequence = manualSequence;
            this.TargetProperty = targetProperty;
        }

        public string Name { get; set; }
        public string Schema { get; set; } = "dbo";
        public bool ManualSequence { get; set; }
        public string TargetProperty { get; set; }

        public DataAccessProviderTypes DataAccessProvider { get; set; } = DataAccessProviderTypes.SqlServer;

    }
}
