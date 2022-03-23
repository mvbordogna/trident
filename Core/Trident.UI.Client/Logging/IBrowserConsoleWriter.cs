
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Trident.UI.Client.Logging
{
    public interface IBrowserConsoleWriter
    {
        Task WriteAsync(LogLevel logLevel, string message);
        void Write(LogLevel logLevel, string formattedMessage);
    }
}
