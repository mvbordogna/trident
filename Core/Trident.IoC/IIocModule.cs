namespace Trident.IoC
{
    public interface IIoCModule
    {
        void Configure(IIoCProvider builder);
    }
}