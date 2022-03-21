using System;

namespace Trident.Data
{
    // https://github.com/Innofactor/EfCoreJsonValueConverter

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class JsonFieldAttribute : Attribute
    {

    }
}
