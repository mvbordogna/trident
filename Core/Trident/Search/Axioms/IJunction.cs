using System.Collections.Generic;

namespace Trident.Search
{
    public interface IJunction
    {
        string GetToken();
        List<Axiom> GetAxioms();
    }
}
