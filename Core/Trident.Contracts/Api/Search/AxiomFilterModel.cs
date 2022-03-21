using System.Collections.Generic;

namespace Trident.Api.Search
{
    public class AxiomFilterModel
    {
        public string Format { get; set; }
        public List<AxiomModel> Axioms { get; set; }
        public AxiomFilterOptions Options { get; set; }
    }
}
