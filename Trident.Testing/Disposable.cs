using System;

namespace Trident.Testing
{
    public abstract class Disposable : IDisposable
    {
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~Disposable()
        {
            Dispose(false);
        }

        protected void Dispose(bool disposing)
        {
            if (disposing)
            {
                DisposeResource();
            }
        }

        protected abstract void DisposeResource();
    }
}