using System.Collections.Generic;

namespace Trident.Search
{
    public class And : ILogicalOperator
    {
        private IJunction parent;

        public And(IJunction parent)
        {
            this.parent = parent;
        }

        List<Axiom> IJunction.GetAxioms()
        {
            return this.parent?.GetAxioms()
                ?? new List<Axiom>();
        }

        string IJunction.GetToken()
        {

            return $"{parent?.GetToken()} & ";
        }
    }
}
