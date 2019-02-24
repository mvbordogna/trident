using Autofac;

namespace Trident.Business.RestSharp
{
    /// <summary>
    /// Class PackageModule.
    /// Implements the <see cref="Autofac.Module" />
    /// </summary>
    /// <seealso cref="Autofac.Module" />
    public class PackageModule : Module
    {
        /// <summary>
        /// Override to add registrations to the container.
        /// </summary>
        /// <param name="builder">The builder through which components can be
        /// registered.</param>
        /// <remarks>Note that the ContainerBuilder parameter is unique to this module.</remarks>
        protected override void Load(ContainerBuilder builder)
        {
            // register auto mapper maps in this assembly
            builder.RegisterAssemblyTypes(this.GetType().Assembly)
                .Where(x => !x.IsAbstract && typeof(AutoMapper.Profile).IsAssignableFrom(x))
                .SingleInstance()
                .As<AutoMapper.Profile>();
        }
    }
}
