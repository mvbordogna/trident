using System;
using Trident.IoC;

namespace Trident.Testing.TestScopes
{
    public class DisposableTestScope<T> : Disposable where T : IDisposable
    {
        private AutofacIoCProvider _iocProvider;

        public T InstanceUnderTest { get; protected set; }

        protected override void DisposeResource()
        {
            // works fine if null
            using (InstanceUnderTest as IDisposable) { }
            using (_iocProvider) { }
        }

        protected IIoCServiceLocator BuildWebIoCContainer(Action<IIoCProvider> customOverrides = null)
        {
            //TODO: uncomment the below to truly make this identical to what Web does
            _iocProvider = new AutofacIoCProvider();

            _iocProvider
                .RegisterSelf()
                //.RegisterOwinMiddleware<ClaimsSecurityOwinMiddleware>()
                .RegisterModules($@"{System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)}\autofac.json");



            customOverrides?.Invoke(_iocProvider);

            _iocProvider.Build();
            return _iocProvider.Get<IIoCServiceLocator>();
        }
    }
}