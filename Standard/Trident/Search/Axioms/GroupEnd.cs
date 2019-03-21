using System.Collections.Generic;

namespace Trident.Search
{
    public class GroupEnd : IJunction
    {
        private IJunction parent;

        public GroupEnd(IJunction parent)
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

            return $"{parent?.GetToken()}) ";
        }
    }  
}
