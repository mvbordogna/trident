using System.IO;

namespace Trident.Domain
{
    /// <summary>
    /// A data class that represents a File Storage Entity.
    /// Implements the <see cref="TridentOptionsBuilder.Domain.EntityBase{System.String}" />
    /// </summary>
    /// <seealso cref="TridentOptionsBuilder.Domain.EntityBase{System.String}" />
    public class FileStorageEntity : EntityBase<string>
    {
        /// <summary>
        /// Gets the storage key.
        /// </summary>
        /// <value>The storage key.</value>
        public string StorageKey
        {
            get
            {
                return this.Id;
            }
        }

        /// <summary>
        /// Gets or sets the source.
        /// </summary>
        /// <value>The source.</value>
        public Stream Source { get; set; }

        /// <summary>
        /// Gets or sets the size.
        /// </summary>
        /// <value>The size.</value>
        public long Size { get; set; }

        /// <summary>
        /// Gets or sets the destination folder.
        /// </summary>
        /// <value>The destination folder.</value>
        public string DestinationFolder { get; set; }

        /// <summary>
        /// Gets or sets the extension.
        /// </summary>
        /// <value>The extension.</value>
        public string Extension { get; set; }

        /// <summary>
        /// Gets or sets the name of the file.
        /// </summary>
        /// <value>The name of the file.</value>
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="FileStorageEntity"/> is encrypted.
        /// </summary>
        /// <value><c>true</c> if encrypted; otherwise, <c>false</c>.</value>
        public bool Encrypted { get; set; }
    }
}
