namespace Trident.Search
{
    public class AxiomFilterBuilder
    {
        public static AxiomFilterBuilder CreateFilter()
        {
            return new AxiomFilterBuilder();
        }
    }

    public static class AxiomFilterBuilderExtensoins
    {
        public static AxiomFilter BuildString(this AxiomTokenizer last)
        {
            var end = ((IJunction)last);
            return new AxiomFilter()
            {
                Format = end.GetToken(),
                Axioms = end.GetAxioms(),
                Options = AxiomFilterOptions.Equation
            };
        }

        public static AxiomFilter Build(this GroupEnd last)
        {
            var end = ((IJunction)last);
            return new AxiomFilter()
            {
                Format = end.GetToken(),
                Axioms = end.GetAxioms(),
                Options = AxiomFilterOptions.Equation
            };
        }

        public static AxiomTokenizer AddAxiom(this AxiomFilterBuilder left, Axiom axiom)
        {
            return new AxiomTokenizer(null, axiom);
        }

        public static GroupStart StartGroup(this AxiomFilterBuilder left)
        {
            return new GroupStart();
        }

        public static GroupStart StartGroup(this GroupStart parent)
        {
            return new GroupStart(parent);
        }

        public static GroupStart StartGroup(this Not parent)
        {
            return new GroupStart(parent);
        }

        public static GroupStart StartGroup(this ILogicalOperator parent)
        {
            return new GroupStart(parent);
        }


        public static AxiomTokenizer AddAxiom(this GroupStart parent, Axiom axiom)
        {
            return new AxiomTokenizer(parent, axiom);
        }


        public static GroupEnd EndGroup(this AxiomTokenizer last)
        {
            return new GroupEnd(last);
        }

        public static GroupEnd EndGroup(this GroupEnd last)
        {
            return new GroupEnd(last);
        }

        public static ILogicalOperator And(this GroupEnd left)
        {
            return new And(left);

        }

        public static ILogicalOperator Or(this GroupEnd left)
        {
            return new Or(left);
        }

        public static ILogicalOperator And(this AxiomTokenizer left)
        {
            return new And(left);

        }

        public static ILogicalOperator Or(this AxiomTokenizer left)
        {
            return new Or(left);
        }

        public static AxiomTokenizer AddAxiom(this Not parent, Axiom axiom)
        {
            return new AxiomTokenizer(parent, axiom);
        }

        public static AxiomTokenizer AddAxiom(this ILogicalOperator parent, Axiom axiom)
        {
            return new AxiomTokenizer(parent, axiom);
        }


        public static Not Not(this Not parent)
        {
           return new Not(parent);
        }
        public static Not Not(this ILogicalOperator parent)
        {
           return new Not(parent);
        }

        public static Not Not(this GroupStart parent)
        {
            return new Not(parent);
        }

        public static Not Not(this GroupEnd parent)
        {
            return new Not(parent);
        }

        public static Not Not(this AxiomFilterBuilder parent)
        {
            return new Not(null);
        }

    }
}
