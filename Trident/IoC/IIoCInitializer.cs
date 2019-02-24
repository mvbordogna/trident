namespace Trident.IoC
{
    /// <summary>
    /// Interface IIoCInitializer
    /// </summary>
    public interface IIoCInitializer
    {
        /// <summary>
        /// Gets the service locator.
        /// </summary>
        /// <returns>IIoCServiceLocator.</returns>
        IIoCServiceLocator GetServiceLocator();
    }
}