namespace Trident.Search
{
    public class Compare
    {
        public object Value { get; set; }
        public CompareOperators Operator { get; set; }
        public bool IgnoreCase { get; set; } = true;
    }

}
