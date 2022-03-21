using System;

namespace Trident.Data
{

    [System.AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class ContainerAttribute : Attribute
    {
        // This is a positional argument
        public ContainerAttribute(string name, string partitionKey, string partitionKeyValue)
        {
            this.Name = name;
            this.PartitionKey = partitionKey;
            this.PartitionKeyValue = partitionKeyValue;
        }

        // This is a named argument
        public string Name { get; set; }

        public string PartitionKey { get; set; }
        public string PartitionKeyValue { get; set; }
    }


}
