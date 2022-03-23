using System;
using System.Collections.Generic;
using System.Text;

namespace Trident.Api.Search
{
    public class AxiomModel : CompareModel
    {

        public AxiomModel() { }

        public AxiomModel(CompareModel compare, string field, string key = null)
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
}
