using System.Collections.Generic;

namespace Trident.IoC
{
    public interface IRegistrationModule
    {
        List<ITypeRegistration> GetRegistrations();

        List<ITypeRegistration> GetScanRegistrations();
    }
}