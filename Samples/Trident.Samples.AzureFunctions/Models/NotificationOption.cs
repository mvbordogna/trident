using Newtonsoft.Json;

namespace Trident.Samples.AzureFunctions.Models
{
    public class NotificationOption
    {
        public string Name { get; set; }
        public string Display { get; set; }
        public bool Required { get; set; }
    }
}
