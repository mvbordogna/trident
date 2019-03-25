using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Trident.Search.Axioms;

namespace Trident.Search
{
    public class AxiomFilter
    {
        public string Format { get; set; }
        public List<Axiom> Axioms { get; set; }
        public AxiomFilterOptions Options { get; set; }
    }


    internal static class AxiomFilterExtensions
    {
        const string AND = "&";
        const string AND_ALSO = "&&";
        const string OR_ELSE = "||";
        const string OR = "|";
        const string NOT = "!";

        static Dictionary<CompareOperators, Func<Expression, Expression, BinaryExpression>> _operatorDict
            = new Dictionary<CompareOperators, Func<Expression, Expression, BinaryExpression>>()
            {
                { CompareOperators.eq,  Expression.Equal },
                { CompareOperators.ne,  Expression.NotEqual },
                { CompareOperators.gt,  Expression.GreaterThan },
                { CompareOperators.gte,  Expression.GreaterThanOrEqual },
                { CompareOperators.lt,  Expression.LessThan },
                { CompareOperators.lte,  Expression.LessThanOrEqual },
                { CompareOperators.contains,  (a,b) => TypeExtensions. GetStringEvalOperationExpression(nameof(String.Contains), a, b) },
                { CompareOperators.startsWith,  (a,b) => TypeExtensions. GetStringEvalOperationExpression(nameof(String.StartsWith), a, b) },
                { CompareOperators.endWith,  (a,b) => TypeExtensions. GetStringEvalOperationExpression(nameof(String.EndsWith), a, b) }
            };

        public static Expression<Func<T, bool>> ToExpression<T>(this AxiomFilter filter)
        {
            var axDict = GetAxiomExpressionDictionary<T>(filter);
            filter.Format = filter.Format.Replace(AND_ALSO, AND).Replace(OR_ELSE, OR);

            var junctionDict = new Dictionary<string, Func<Expression<Func<T, bool>>, Expression<Func<T, bool>>, Expression<Func<T, bool>>>>()
                 {
                    {AND, TypeExtensions.AndAlso },
                    {OR, TypeExtensions.OrElse }
                 };

            Func<string, bool> IsBinOperator = (string s) =>
            {
                return (s == AND || s == OR);
            };

            Func<string, bool> IsUnaryOperator = (string s) =>
            {
                return (s == NOT);
            };

            Func<object, Expression<Func<T, bool>>> GetAxiomFunc = (object o) =>
            {
                if (!(o is Token))
                    return o as Expression<Func<T, bool>>;

                return axDict[((Token)o).Value];
            };


            using (var reader = new StringReader(filter.Format))
            {
                var parser = new Parser();
                var tokens = parser.Tokenize(reader).ToList();
                var rpn = parser.ShuntingYard(tokens).ToList();
                var processStack = new Stack<object>();
                var i = 0;

                do
                {
                    var t = rpn[i];
                    if (IsBinOperator(t.Value))
                    {
                        processStack.Push(
                            junctionDict[t.Value](
                                GetAxiomFunc(processStack.Pop()),
                                GetAxiomFunc(processStack.Pop())
                            )
                        );
                    }
                    else if (IsUnaryOperator(t.Value))
                    {

                        processStack.Push(
                             TypeExtensions.Not(
                                GetAxiomFunc(processStack.Pop())
                            )
                        );
                    }
                    else
                    {
                        processStack.Push(t);
                    }

                    i++;
                }
                while (i < rpn.Count);

                var exp = processStack.Pop() as Expression<Func<T, bool>>;

                return (exp.CanReduce)
                    ? exp.Reduce() as Expression<Func<T, bool>>
                    : exp;
            }
        }


        public static Expression<Func<T, bool>> ToExpression<T>(this Axiom axiom, ParameterExpression paramExpression = null)
        {
            var type = typeof(T);
            paramExpression = paramExpression ?? Expression.Parameter(type, "🖕🏽");
            var a = axiom;
            var fieldName = a.Field;

            var property = type.GetProperties(BindingFlags.Public | BindingFlags.Instance).FirstOrDefault(x => x.Name == fieldName);
            if (property == null)
                throw new ArgumentException($"{fieldName} is not a valid member of {type.FullName}");

            var keyPropertyExpression = Expression.Property(paramExpression, property);
            var compareExpressionFunc = _operatorDict[a.Operator];

            var filterValue = ResolveTypeBoxing(keyPropertyExpression.Type, a.Value);
            var constantExpression = Expression.Constant(filterValue);

            var equalExpression = !TypeExtensions.IsNullableMember(keyPropertyExpression.Type)
                ? compareExpressionFunc(keyPropertyExpression, constantExpression)
                : TypeExtensions.NullableComparision(compareExpressionFunc, keyPropertyExpression, constantExpression);

            return Expression.Lambda<Func<T, bool>>(equalExpression, paramExpression);
        }


        private static Dictionary<string, Expression<Func<T, bool>>> GetAxiomExpressionDictionary<T>(AxiomFilter filter)
        {
            var type = typeof(T);
            var axiomDict = new Dictionary<string, Expression<Func<T, bool>>>();
            var paramExpression = Expression.Parameter(type, "🖕🏽");

            foreach (var a in filter.Axioms)
            {
                var conditionalExpression = a.ToExpression<T>(paramExpression);
                axiomDict.Add($"{{{a.Key}}}", conditionalExpression);
            }

            return axiomDict;
        }

        private static object ResolveTypeBoxing(Type targetType, object value)
        {
            var filterValueType = value.GetType();
            if (filterValueType == typeof(string))
            {
                var typeDelegate = TypeExtensions.GetParserFunction(targetType);
                return typeDelegate(value as string);
            }
            else if (filterValueType.IsPrimitive())
            {
                return value;
            }
            else throw new NotSupportedException("Complex filter values are only supported using ComplexFilterStrategies.");
        }
    }
}
