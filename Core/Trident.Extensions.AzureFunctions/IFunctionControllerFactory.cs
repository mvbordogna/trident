using System;

namespace Scholar.Framework.Azure.Common
{
    public interface IFunctionControllerFactory
    {
        Type GetControllerType(string fullyQualifiedName);
    }
}
