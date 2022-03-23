using System;

namespace Trident.UI.Client
{
    [AttributeUsage(System.AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    public class ServiceAttribute : Attribute
    {
        public ServiceAttribute(string name, string resourceName = null)
        {
            this.Name = name;
            this.ResourceName = resourceName;
        }

        public string Name { get; }
        public string ResourceName { get; }
    }
}
