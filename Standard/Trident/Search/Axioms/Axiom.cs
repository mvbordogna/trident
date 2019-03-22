namespace Trident.Search
{
    public class Axiom : Compare
    {

        public Axiom()        {        }

        public Axiom(Compare compare, string field, string key = null)
        {
            this.Operator = compare.Operator;
            this.Value = compare.Value;
            Field = field;
            Key = key;
        }
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
