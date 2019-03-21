using System.Collections.Generic;

namespace Trident.Search
{
    public class Or : ILogicalOperator
    {
        private IJunction parent;

        public Or(IJunction parent)
        {
            this.parent = parent;
        }

        string IJunction.GetToken()
        {
            return $"{parent?.GetToken()} | ";
        }

        List<Axiom> IJunction.GetAxioms()
        {
            return this.parent?.GetAxioms()
                ?? new List<Axiom>();
        }
    }
}
