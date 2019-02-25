namespace Trident.Domain
{
    /// <summary>
    /// Class EmailTemplate data class.
    /// </summary>
    public class EmailTemplate
    {
        //
        // Summary:
        //     When sending emails, defines a subject.
        /// <summary>
        /// Gets or sets the subject.
        /// </summary>
        /// <value>The subject.</value>
        public string Subject { get; set; }
        //
        // Summary:
        //     View file location (virtual path '~/')
        /// <summary>
        /// Gets or sets the location.
        /// </summary>
        /// <value>The location.</value>
        public string Location { get; set; }
    }
}
