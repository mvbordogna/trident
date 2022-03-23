using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trident.UI.Blazor.Logging.AppInsights
{
    public class Error
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("stack")]
        public string Stack { get; set; }
    }
}
