using System;
using Microsoft.Azure.WebJobs.Description;

namespace Trident.Azure
{
    /// <summary>
    /// Class InjectAttribute.
    /// Implements the <see cref="System.Attribute" />
    /// </summary>
    /// <seealso cref="System.Attribute" />
    [AttributeUsage(AttributeTargets.Parameter)]
    [Binding]
    public class InjectAttribute : System.Attribute
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }
        /// <summary>
        /// Gets a value indicating whether this instance has name.
        /// </summary>
        /// <value><c>true</c> if this instance has name; otherwise, <c>false</c>.</value>
        public bool HasName => !string.IsNullOrWhiteSpace(Name);

        /// <summary>
        /// Initializes a new instance of the <see cref="InjectAttribute"/> class.
        /// </summary>
        public InjectAttribute()
        {

        }
        /// <summary>
        /// Initializes a new instance of the <see cref="InjectAttribute"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public InjectAttribute(string name)
        {
            Name = name;
        }
    }
}