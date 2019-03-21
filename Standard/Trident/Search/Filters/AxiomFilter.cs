using System.Collections.Generic;

namespace Trident.Search
{
    public class AxiomFilter
    {
        public string Format { get; set; }
        public List<Axiom> Axioms { get; set; }
        public AxiomFilterOptions Options { get; set; }
    }

}
