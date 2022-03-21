namespace Trident.IoC
{
    public enum LifeSpan
    {
        InstancePerLifetimeScope = 0,
        SingleInstance = 1,
        NewInstancePerRequest = 2,
        ExternallyOwned = 3
    }
}
