using System.Reflection;
using Moq;
using Trident.IoC;

namespace Trident.Testing
{
    public static class IoCHelper
    {
        static IoCHelper()
        {
            var bindings = BindingFlags.Static |
             BindingFlags.NonPublic;
            IoCMock = new Mock<IIoCProvider>();
            var instanceField = typeof(AutofacIoCProvider).GetField("instance", bindings);
            instanceField.SetValue(null, IoCMock.Object);
            IoCMock.Setup(x => x.Get<IIoCProvider>()).Returns(IoCMock.Object);
        }

        public static Mock<IIoCProvider> IoCMock { get; private set; }
    }
}
