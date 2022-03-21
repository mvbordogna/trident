using Autofac;
using S = Serilog;

namespace Trident.Logging.Serilog
{
    /// <summary>
    /// Trident.Logging.Serilog PackageModule Class.
    /// Implements the <see cref="Autofac.Module" />
    /// </summary>
    /// <seealso cref="Autofac.Module" />
    public class PackageModule:Module
    {
        /// <summary>
        /// Override to add registrations to the container.
        /// </summary>
        /// <param name="builder">The builder through which components can be
        /// registered.</param>
        /// <remarks>Note that the ContainerBuilder parameter is unique to this module.</remarks>
        protected override void Load(ContainerBuilder builder)
        {
            builder
            .RegisterInstance(S.Log.Logger)
            .As<S.ILogger>()
            .SingleInstance();

            builder.RegisterType<SeriLogger>()
                .As<ILog>()
                .SingleInstance();
        }
    }
}
