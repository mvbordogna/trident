using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Trident.Configuration
{
    public class StreamFileProvider : IFileProvider
    {
        private IEnumerable<IFileInfo> _files;
        private Dictionary<string, IChangeToken> _changeTokens;

        public StreamFileProvider() { }

        public StreamFileProvider(params IFileInfo[] files)
        {
            _files = files;
        }

        public StreamFileProvider(params KeyValuePair<string, IChangeToken>[] changeTokens)
        {
            _changeTokens = changeTokens.ToDictionary(
                changeToken => changeToken.Key,
                changeToken => changeToken.Value,
                StringComparer.Ordinal);
        }

        public IDirectoryContents GetDirectoryContents(string path)
        {
            IEnumerable<IFileInfo> filesInFolder = _files.Where(f => f.Name.StartsWith(path, StringComparison.Ordinal));

            if (filesInFolder.Any())
            {
                return new StreamDirectoryContents(filesInFolder);
            }

            return NotFoundDirectoryContents.Singleton;
        }

        public IFileInfo GetFileInfo(string path)
        {
            IFileInfo file = _files.FirstOrDefault(f => f.Name == path);
            return file ?? new NotFoundFileInfo(path);
        }

        public IChangeToken Watch(string filter)
        {
            if (_changeTokens != null && _changeTokens.ContainsKey(filter))
            {
                return _changeTokens[filter];
            }

            return NullChangeToken.Singleton;
        }
    }
}
