using System.Transactions;

namespace Trident.Transactions
{
    /// <summary>
    /// Interface ITransactionScopeFactory
    /// </summary>
    public interface ITransactionScopeFactory
    {
        /// <summary>
        /// Gets the transaction scope.
        /// </summary>
        /// <returns>TransactionScope.</returns>
        TransactionScope GetTransactionScope();
    }
}
