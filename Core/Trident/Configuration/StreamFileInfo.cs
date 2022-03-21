using Microsoft.Extensions.FileProviders;
using System;
using System.IO;

namespace Trident.Configuration
{

    public class StreamFileInfo : IFileInfo
    {
        private readonly Stream _content;

        public StreamFileInfo(string name, Stream stream, DateTimeOffset? timestamp = null)
        {
            Name = name;
            _content = stream;
            LastModified = timestamp ?? DateTimeOffset.Now;
        }

        public bool Exists => true;
        public long Length => _content.Length;
        public string PhysicalPath => null;
        public string Name { get; }
        public DateTimeOffset LastModified { get; }
        public bool IsDirectory => false;


        public Stream CreateReadStream()
        {
            return _content;
        }
    }
}
