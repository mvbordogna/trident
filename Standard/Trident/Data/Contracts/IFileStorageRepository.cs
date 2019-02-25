﻿using Trident.Domain;
using System.Threading.Tasks;

namespace Trident.Data.Contracts
{
    /// <summary>
    /// Interface IFileStorageRepository
    /// Implements the <see cref="Trident.Data.Contracts.IRepositoryBase{Trident.Domain.FileStorageEntity}" />
    /// </summary>
    /// <seealso cref="Trident.Data.Contracts.IRepositoryBase{Trident.Domain.FileStorageEntity}" />
    public interface IFileStorageRepository : IRepositoryBase<FileStorageEntity>
    {

        /// <summary>
        /// Existses the specified filename.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        Task<bool> Exists(string filename);
    }
}
