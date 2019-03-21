namespace Trident.Search
{
    public class Axiom : Compare
    {
        private string key = null;

        public string Field { get; set; }

        public string Key
        {
            get { return key ?? Field; }
            set { key = value; }
        }
    }

    public class Axiom<T> : Axiom { }

}
