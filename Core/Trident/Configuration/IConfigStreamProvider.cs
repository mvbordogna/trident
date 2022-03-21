using System.IO;
using System.Threading.Tasks;

namespace Trident.Configuration
{
    public interface IConfigStreamProvider
    {
        Task<Stream> GetStream(string filename);
    }
}
