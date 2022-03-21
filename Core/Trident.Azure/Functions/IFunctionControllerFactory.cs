using System;

namespace Trident.Azure.Functions
{
    public interface IFunctionControllerFactory
    {
        Type GetControllerType(string fullyQualifiedName);
    }
}
