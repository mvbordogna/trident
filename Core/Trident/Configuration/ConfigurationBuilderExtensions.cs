using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Trident.Configuration
{

    public static class ConfigurationBuilderExtensions
    {
        /// <summary>
        /// Adds the specified JSON into the IConfigurationBuilder as if adding a real JSON file.
        /// </summary>
        /// <param name="configurationBuilder"></param>
        /// <param name="inMemoryJson">The JSON data to use in the configuration.</param>
        /// <returns></returns>
        public static IConfigurationBuilder AddInMemoryJson(this IConfigurationBuilder configurationBuilder, string fileName,  IConfigStreamProvider streamProvider = null)
        {
            streamProvider = streamProvider ?? new DefaultConfigStreamProvider();
            Stream stream = null;

            Task.Run(async () =>
            {
                stream = await streamProvider.GetStream(fileName);
            }).Wait();

    
            var fileInfo = new StreamFileInfo(fileName, stream, DateTimeOffset.Now);
            var @in = new StreamFileProvider(fileInfo);
            return configurationBuilder.AddJsonFile(@in, fileName, optional: false, reloadOnChange: false);
        }
    }
}
