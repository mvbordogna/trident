using System.Collections.Generic;

namespace Trident.Search
{
    public class GroupStart : IJunction
    {
        private IJunction parent;

        public GroupStart()
        {
        }

        public GroupStart(IJunction parent)
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
            return $"{parent?.GetToken()} (";
        }
    }
}
