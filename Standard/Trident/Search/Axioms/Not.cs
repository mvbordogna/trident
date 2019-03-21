using System.Collections.Generic;

namespace Trident.Search
{
    public class Not : IJunction
    {
        private IJunction parent;

        public Not(IJunction parent)
        {
            this.parent = parent;
        }

        string IJunction.GetToken()
        {
            return $"{parent?.GetToken()} !";
        }

        List<Axiom> IJunction.GetAxioms()
        {
            return this.parent?.GetAxioms()
                ?? new List<Axiom>();
        }
    }
}
