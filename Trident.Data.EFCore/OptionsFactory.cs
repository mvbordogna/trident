﻿using Microsoft.EntityFrameworkCore;
using Trident.IoC;
using Trident.Data.EntityFramework.EFCore.Contracts;

namespace Trident.Data.EntityFramework.EFCore
{
    /// <summary>
    /// Class OptionsFactory.
    /// Implements the <see cref="Trident.Data.EntityFramework.EFCore.Contracts.IOptionsFactory" />
    /// </summary>
    /// <seealso cref="Trident.Data.EntityFramework.EFCore.Contracts.IOptionsFactory" />
    public class OptionsFactory : IOptionsFactory
    {
        /// <summary>
        /// The service locator
        /// </summary>
        private readonly IIoCServiceLocator _serviceLocator;

        /// <summary>
        /// Initializes a new instance of the <see cref="OptionsFactory"/> class.
        /// </summary>
        /// <param name="serviceLocator">The service locator.</param>
        public OptionsFactory(IIoCServiceLocator serviceLocator)
        {
            _serviceLocator = serviceLocator;
        }

        /// <summary>
        /// Gets the options.
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        /// <returns>DbContextOptions.</returns>
        public DbContextOptions GetOptions(string dataSource)
        {
            var builder = _serviceLocator.GetNamed<IOptionsBuilder>(dataSource);
            return builder.GetOptions(dataSource);
        }
    }
}
