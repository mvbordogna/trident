using System;

namespace Trident.Extensions.OpenApi.Attributes
{
    /// <summary>
    /// This represents the attribute entity for HTTP triggers to define payload. This MUST be inherited.
    /// </summary>
    public abstract class OpenApiPayloadAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OpenApiPayloadAttribute"/> class.
        /// </summary>
        /// <param name="contentType">Content type.</param>
        /// <param name="bodyType">Type of payload.</param>
        protected OpenApiPayloadAttribute(string contentType, Type bodyType)
        {
            this.ContentType = contentType ?? throw new ArgumentNullException(nameof(contentType));
            this.BodyType = bodyType ?? throw new ArgumentNullException(nameof(bodyType));
        }

        /// <summary>
        /// Gets the content type.
        /// </summary>
        public virtual string ContentType { get; }

        /// <summary>
        /// Gets the payload body type.
        /// </summary>
        public virtual Type BodyType { get; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        public virtual string Description { get; set; }
    }
}
