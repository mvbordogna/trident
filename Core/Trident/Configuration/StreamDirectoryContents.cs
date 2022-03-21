using Microsoft.Extensions.FileProviders;
using System.Collections;
using System.Collections.Generic;

namespace Trident.Configuration
{
    public class StreamDirectoryContents : IDirectoryContents
    {
        private IEnumerable<IFileInfo> _files;

        public StreamDirectoryContents(IEnumerable<IFileInfo> files)
        {
            _files = files;
        }

        public bool Exists => true;

        public IEnumerator<IFileInfo> GetEnumerator()
        {
            return _files.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _files.GetEnumerator();
        }
    }
}
