namespace Trident.Testing.TestScopes
{
    public class TestScope<T> : ITestScope<T> where T : class
    {
        public T InstanceUnderTest { get; set; }

    }

    public interface ITestScope<T>
    {
        T InstanceUnderTest { get; set; }
    }
}