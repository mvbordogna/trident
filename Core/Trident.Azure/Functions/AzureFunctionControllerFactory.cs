using System.Linq;
using System.Collections.Generic;
using System;

namespace Trident.Azure.Functions
{
    public class AzureFunctionControllerFactory : IFunctionControllerFactory


    {
        private Dictionary<string, Type> _controllerTypes;

        public AzureFunctionControllerFactory(IEnumerable<Type> controllerTypes)
        {
            _controllerTypes = controllerTypes?.ToDictionary(x => x.FullName);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="fullyqualifyedName">Namespace and class name without assembly quailification</param>
        public Type GetControllerType(string fullyqualifyedName)
        {
            return (_controllerTypes.ContainsKey(fullyqualifyedName))
                ? _controllerTypes[fullyqualifyedName]
                : null;
        }
    }
}
