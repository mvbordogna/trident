using System;

namespace Trident.EFCore
{
    public class DataSourceInfo
    {
        public string DatabaseName { get; internal set; }
        public string Container { get; internal set; }
        public string PartitionKey { get; internal set; }
        public string DiscriminatorProperty { get; internal set; }
        public string DiscriminatorValue { get; internal set; }
        public Type TargetEntityType { get; internal set; }
    }
}
