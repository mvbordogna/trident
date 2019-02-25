using Autofac.Extras.Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trident.Testing.TestScopes
{
    public class AutoMockTestScope<T> : TestScope<T>, IAutoMockTestScope where T : class
    {
        public AutoMock ResolverInstance { get; set; }

        public virtual void Initialize()
        {
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
            if (ResolverInstance != null) ResolverInstance.Dispose();
        }
        #endregion
    }

    public interface IAutoMockTestScope : IDisposable
    {
        AutoMock ResolverInstance { get; set; }
   
        void Initialize();

    }
}
