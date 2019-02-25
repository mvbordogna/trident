using System.Transactions;

namespace Trident.Testing.TestScopes
{
    public class AutoRollbackTransactionScope : Disposable 
    {
        private readonly TransactionScope _transactionScope = new TransactionScope();


        protected override void DisposeResource()
        {
            using (_transactionScope){}
        }
    }
}