using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Trident.Configuration
{
    /// <summary>
    /// this Config Stream provider works on PC/Mac with normal unrestructed File Access
    /// </summary>
    public class DefaultConfigStreamProvider : IConfigStreamProvider
    {
        public Task<Stream> GetStream(string filename)
        {
            var stream = File.Open(filename, FileMode.Open, FileAccess.Read);
            AdvanceStreamPastBOM(stream);
            return Task.FromResult(stream as Stream);
        }

        private static void AdvanceStreamPastBOM(Stream stream)
        {
            if (stream.Length < 3) return;
            var bom = new byte[] { 0xEF, 0xBB, 0xBF };
            var buffer = new byte[3];
            stream.Read(buffer, 0, 3);

            if (!buffer.SequenceEqual(bom))
                stream.Position = 0;
        }
    }

}
