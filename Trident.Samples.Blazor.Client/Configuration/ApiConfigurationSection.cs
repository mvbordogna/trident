namespace Trident.Samples.Blazor.Client.Configuration
{
    public class ApiConfigurationSection
    {
        public string BaseUrl { get; set; }
        public string Scopes { get; set; }
        public string UserFlow { get; set; }
        public bool? UseMockData { get; set; }
    }
}
