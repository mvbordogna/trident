using Trident.Common;
using System;
using System.Transactions;
using Trident.Contracts.Configuration;

namespace Trident.Transactions
{
    /// <summary>
    /// Class TransactionScopeFactory.
    /// Implements the <see cref="TridentOptionsBuilder.Transactions.ITransactionScopeFactory" />
    /// </summary>
    /// <seealso cref="TridentOptionsBuilder.Transactions.ITransactionScopeFactory" />
    public class TransactionScopeFactory : ITransactionScopeFactory
    {
        /// <summary>
        /// The application settings
        /// </summary>
        private readonly IAppSettings _appSettings;
        /// <summary>
        /// The scope option
        /// </summary>
        private readonly TransactionScopeOption _scopeOption = TransactionScopeOption.Required;

        /// <summary>
        /// Initializes a new instance of the <see cref="TransactionScopeFactory"/> class.
        /// </summary>
        /// <param name="appSettings">The application settings.</param>
        public TransactionScopeFactory(IAppSettings appSettings) {
            _appSettings = appSettings;
            _scopeOption = GetTransactionScopeOption();
        }

        /// <summary>
        /// Gets the transaction scope option.
        /// </summary>
        /// <returns>TransactionScopeOption.</returns>
        private TransactionScopeOption GetTransactionScopeOption()
        {          
            TransactionScopeOption temp = TransactionScopeOption.Required;
            Enum.TryParse(_appSettings["TransactionScopeOption"] ?? "Required", out temp);
            return temp;
        }

        /// <summary>
        /// Gets the transaction scope.
        /// </summary>
        /// <returns>TransactionScope.</returns>
        public TransactionScope GetTransactionScope()
        {
            var option = new TransactionOptions()
            {
                IsolationLevel = IsolationLevel.ReadCommitted,
                Timeout = TimeSpan.Parse(_appSettings["TransactionScopeTimeout"])
            };
            return new TransactionScope(_scopeOption, option, TransactionScopeAsyncFlowOption.Enabled);
        }
    }
}
