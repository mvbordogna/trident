using Trident.IoC;
using Trident.EFCore.Contracts;

namespace Trident.EFCore
{
    /// <summary>
    /// Class EFCoreModelBuilderFactory.
    /// Implements the <see cref="Trident.EFCore.Contracts.IEFCoreModelBuilderFactory" />
    /// </summary>
    /// <seealso cref="Trident.EFCore.Contracts.IEFCoreModelBuilderFactory" />
    public class EFCoreModelBuilderFactory : IEFCoreModelBuilderFactory
    {
        /// <summary>
        /// The service locator
        /// </summary>
        private readonly IIoCServiceLocator _serviceLocator;

        /// <summary>
        /// Initializes a new instance of the <see cref="EFCoreModelBuilderFactory"/> class.
        /// </summary>
        /// <param name="serviceLocator">The service locator.</param>
        public EFCoreModelBuilderFactory(IIoCServiceLocator serviceLocator)
        {
            _serviceLocator = serviceLocator;
        }

        /// <summary>
        /// Gets the Model To use when creating the DbContext
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        /// <returns>IEFCoreModelBuilder.</returns>
        public IEFCoreModelBuilder GetBuilder(string dataSource)          
        {
            return _serviceLocator.GetNamed<IEFCoreModelBuilder>(dataSource);
        }
    }
}
