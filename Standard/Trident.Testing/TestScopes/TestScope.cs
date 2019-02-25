namespace Trident.Testing.TestScopes
{
    public class TestScope<T> where T : class
    {
        public T InstanceUnderTest { get; set; }
    }
}