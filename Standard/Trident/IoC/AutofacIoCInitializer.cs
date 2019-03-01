using Trident.IoC;

namespace Trident.IoC
{
    /// <summary>
    /// Class AutofacIoCInitializer.
    /// Implements the <see cref="Trident.IoCInitializer" />
    /// </summary>
    /// <seealso cref="Trident.IoCInitializer" />
    public class AutofacIoCInitializer : IoCInitializer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AutofacIoCInitializer"/> class.
        /// </summary>
        public AutofacIoCInitializer()
            : base(new BootstrapScanner(), new AutofacIoCProvider()) { }
    }
}
