using System;
using System.Collections.Generic;

namespace Trident.IoC
{
    /// <summary>
    /// Class IoCInitializer.
    /// Implements the <see cref="Trident.IIoCInitializer" />
    /// </summary>
    /// <seealso cref="Trident.IIoCInitializer" />
    public class IoCInitializer : IIoCInitializer
    {
        /// <summary>
        /// The ioc service locator
        /// </summary>
        private IIoCServiceLocator _iocServiceLocator;
        /// <summary>
        /// The bootstrap scanner
        /// </summary>
        private readonly IBootstrapScanner _bootstrapScanner;
        /// <summary>
        /// The ioc provider
        /// </summary>
        private readonly IIoCProvider _iocProvider;
        /// <summary>
        /// The pad lock
        /// </summary>
        private static object pad_lock = new object();

        /// <summary>
        /// Initializes a new instance of the <see cref="IoCInitializer"/> class.
        /// </summary>
        /// <param name="bootstrapScanner">The bootstrap scanner.</param>
        /// <param name="iocProvider">The ioc provider.</param>
        public IoCInitializer(IBootstrapScanner bootstrapScanner, IIoCProvider iocProvider)
        {
            _bootstrapScanner = bootstrapScanner;
            _iocProvider = iocProvider;
        }

        /// <summary>
        /// Gets the service locator.
        /// </summary>
        /// <returns>IIoCServiceLocator.</returns>
        public IIoCServiceLocator GetServiceLocator()
        {
            if (_iocServiceLocator == null)
            {
                lock (pad_lock)
                {
                    if (_iocServiceLocator == null)
                    {
                        Initialize();
                    }
                }
            }

            return _iocServiceLocator;
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        protected virtual void Initialize()
        {
            var moduletypes = GetModuleTypes();
            var iocProvider = _iocProvider;
            moduletypes.ForEach(x => iocProvider.RegisterModule(x));           
           iocProvider
                .RegisterSelf()
                .Build();

            _iocServiceLocator = iocProvider.Get<IIoCServiceLocator>();
        }

        /// <summary>
        /// Gets the module types.
        /// </summary>
        /// <returns>List&lt;Type&gt;.</returns>
        /// <exception cref="Trident.BootstrapperNotFoundException">No bootstrapper instances had been recognized.</exception>
        private List<Type> GetModuleTypes()
        {         
            var bootstrappers = _bootstrapScanner.GetBootstrappers();

            if (bootstrappers.Count == 0)
            {
                throw new BootstrapperNotFoundException("No bootstrapper instances had been recognized.");
            }

            var moduleTypes = new List<Type>();
            foreach (var bootstrapper in bootstrappers)
            {
                var instance = (IBootstrapper)Activator.CreateInstance(bootstrapper);
                moduleTypes.AddRange(instance.GetModuleTypes());
            }

            return moduleTypes;
        }
    }
}