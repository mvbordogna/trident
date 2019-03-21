using System;
using System.Collections.Generic;

namespace Trident.Search
{
    public class AxiomTokenizer : IJunction
    {
        private readonly IJunction parent;
        private readonly Axiom axiom;

        public AxiomTokenizer(IJunction parent, Axiom axiom)
        {
            this.parent = parent;
            this.axiom = axiom;
        }       

        List<Axiom> IJunction.GetAxioms()
        {
            var list = this.parent?.GetAxioms()
                ?? new List<Axiom>();

            list.Add(this.axiom);

            return list;
        }

        string IJunction.GetToken()
        {
            return $"{parent?.GetToken()} {{{axiom.Key ?? axiom.Field}}}";
        }
    }
}
