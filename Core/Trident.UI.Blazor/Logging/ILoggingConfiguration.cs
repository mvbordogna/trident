using Microsoft.Extensions.Logging;

namespace Trident.UI.Blazor.Logging
{
    public interface ILoggingConfiguration
    {
        LogLevel LogLevel { get; }
    }

    public class LoggingConfiguration : ILoggingConfiguration
    {
        public LogLevel LogLevel { get; set; }
    }
}
