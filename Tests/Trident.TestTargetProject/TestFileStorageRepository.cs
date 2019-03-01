using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Trident.Domain;

namespace Trident.TestTargetProject
{
    public class TestFileStorageRepository : Data.Contracts.IFileStorageRepository
    {


        public Task Delete(FileStorageEntity entity, bool deferCommit = false)
        {
            throw new NotImplementedException();
        }

        public void DeleteSync(FileStorageEntity entity, bool deferCommit = false)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Exists(string filename)
        {
            throw new NotImplementedException();
        }

        public Task<FileStorageEntity> GetById(object id, bool detach = false)
        {
            throw new NotImplementedException();
        }

        public FileStorageEntity GetByIdSync(object id, bool detach = false)
        {
            throw new NotImplementedException();
        }

        public Task Insert(FileStorageEntity entity, bool deferCommit = false)
        {
            throw new NotImplementedException();
        }

        public void InsertSync(FileStorageEntity entity, bool deferCommit = false)
        {
            throw new NotImplementedException();
        }

        public Task Update(FileStorageEntity entity, bool deferCommit = false)
        {
            throw new NotImplementedException();
        }

        public void UpdateSync(FileStorageEntity entity, bool deferCommit = false)
        {
            throw new NotImplementedException();
        }
    }
}
