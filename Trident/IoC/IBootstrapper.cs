using System;

namespace Trident.IoC
{
    /// <summary>
    /// Interface IBootstrapper
    /// </summary>
    public interface IBootstrapper
    {
        /// <summary>
        /// Gets the module types.
        /// </summary>
        /// <returns>Type[].</returns>
        Type[] GetModuleTypes();
    }
}
