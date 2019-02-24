using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trident.Transactions
{
    /// <summary>
    /// Class NonTransactionalAttribute. This class cannot be inherited.
    /// Use this attribute to mark a Transaction Proxy wrapped Manager Method so that it is not executed in a transaction when direct calls are made to it.
    /// Use at a class level to apply to all methods of the manager.
    /// </summary>
    /// <seealso cref="System.Attribute" />
    [System.AttributeUsage(AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Interface, Inherited = false, AllowMultiple = false)]
    public sealed class NonTransactionalAttribute : Attribute { }
}

