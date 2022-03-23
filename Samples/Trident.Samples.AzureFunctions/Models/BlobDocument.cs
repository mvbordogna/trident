using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trident.Samples.AzureFunctions.Models
{
    public class BlobDocument
    {
        public Guid Id { get; set; }
        public string SourceFileName { get; set; }
        public string GeneratedFileName { get; set; }
        public string Link { get; set; }
        public byte[] FileData { get; set; }
        public string FileType { get; set; }
    }
}
