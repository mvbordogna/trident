namespace Trident.Contracts.Configuration
{
    public sealed class ConnectionStringSettings
    {
        //
        // Summary:
        //     Initializes a new instance of a System.Configuration.ConnectionStringSettings
        //     class.
        public ConnectionStringSettings() { }
        //
        // Summary:
        //     Initializes a new instance of a System.Configuration.ConnectionStringSettings
        //     class.
        //
        // Parameters:
        //   name:
        //     The name of the connection string.
        //
        //   connectionString:
        //     The connection string.
        public ConnectionStringSettings(string name, string connectionString, string providerName = null)
        {
            Name = name;
            ConnectionString = connectionString;
            ProviderName = providerName;
        }

        //
        // Summary:
        //     Gets or sets the connection string.
        //
        // Returns:
        //     The string value assigned to the System.Configuration.ConnectionStringSettings.ConnectionString
        //     property.
        public string ConnectionString { get; set; }

        //
        // Summary:
        //     Gets or sets the System.Configuration.ConnectionStringSettings name.
        //
        // Returns:
        //     The string value assigned to the System.Configuration.ConnectionStringSettings.Name
        //     property.       
        public string Name { get; set; }

        //
        // Summary:
        //     Gets or sets the provider name property.
        //
        // Returns:
        //     Gets or sets the System.Configuration.ConnectionStringSettings.ProviderName property.       
        public string ProviderName { get; set; }

        //
        // Summary:
        //     Returns a string representation of the object.
        //
        // Returns:
        //     A string representation of the object.
        public override string ToString()
        {
            return ConnectionString;
        }
    }
}
