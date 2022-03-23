namespace Trident.Contracts.Constants
{
    /// <summary>
    /// Provides a list of container names
    /// </summary>
    public static partial class Containers
    {
        public const string Infrastructure = nameof(Infrastructure);
        
        /// <summary>
        /// Provides a list of Partition Values
        /// </summary>
        public static class Partitions
        {
            public const string Organizations = nameof(Organizations);
            
        }

        public static class Discriminators
        {
            public const string Organization = nameof(Organization);
            
        }
    }
}
