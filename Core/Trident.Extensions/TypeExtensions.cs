using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Trident.Extensions;
                       
namespace Trident
{
    public static class TypeExtensions
    {
        private static IReadOnlyCollection<Type> _primitiveList;
        private static ConcurrentDictionary<Type, Func<string, object>> parserLookup = new ConcurrentDictionary<Type, Func<string, object>>
            (
                new List<KeyValuePair<Type, Func<string, object>>>()
                {
                     new KeyValuePair<Type, Func<string, object>>( typeof(bool), x => { return bool.Parse(x); }),
                     new KeyValuePair<Type, Func<string, object>>( typeof(Guid), x => { return Guid.Parse(x); }),
                     new KeyValuePair<Type, Func<string, object>>( typeof(DateTime), x => { return DateTime.Parse(x); }),
                     new KeyValuePair<Type, Func<string, object>>( typeof(string), x => { return x; }),
                     new KeyValuePair<Type, Func<string, object>>( typeof(char), x => { return char.Parse(x);    }),
                     new KeyValuePair<Type, Func<string, object>>( typeof(SByte), x => { return SByte.Parse(x);}),
                     new KeyValuePair<Type, Func<string, object>>( typeof(byte), x => { return byte.Parse(x);}),
                     new KeyValuePair<Type, Func<string, object>>( typeof(long), x => { return long.Parse(x); }),
                     new KeyValuePair<Type, Func<string, object>>( typeof(int), x => { return int.Parse(x); }),
                     new KeyValuePair<Type, Func<string, object>>( typeof(short), x => { return short.Parse(x); }),
                     new KeyValuePair<Type, Func<string, object>>( typeof(ushort), x => { return ushort.Parse(x); }),
                     new KeyValuePair<Type, Func<string, object>>( typeof(ulong), x => { return ulong.Parse(x); }),
                     new KeyValuePair<Type, Func<string, object>>( typeof(uint), x => { return uint.Parse(x); }),
                     new KeyValuePair<Type, Func<string, object>>( typeof(float), x => { return float.Parse(x); }),
                     new KeyValuePair<Type, Func<string, object>>( typeof(double), x => { return double.Parse(x); }),
                     new KeyValuePair<Type, Func<string, object>>( typeof(decimal), x => { return decimal.Parse(x); }),
                     new KeyValuePair<Type, Func<string, object>>( typeof(DateTimeOffset), x => { return DateTimeOffset.Parse(x); }),
                     new KeyValuePair<Type, Func<string, object>>( typeof(TimeSpan), x => { return TimeSpan.Parse(x); }),

                     new KeyValuePair<Type, Func<string, object>>( typeof(bool?), x => { return bool.TryParse(x, out var p) ? (bool?) p : null; }),
                     new KeyValuePair<Type, Func<string, object>>( typeof(Guid?), x => { return Guid.TryParse(x, out var p) ? (Guid?) p : null; }),
                     new KeyValuePair<Type, Func<string, object>>( typeof(DateTime?), x => { return DateTime.TryParse(x, out var p) ? (DateTime?) p : null; }),
                     new KeyValuePair<Type, Func<string, object>>( typeof(char?), x => { return char.TryParse(x, out var p) ? (char?) p : null; }),
                     new KeyValuePair<Type, Func<string, object>>( typeof(SByte?), x => { return SByte.TryParse(x, out var p) ? (SByte?) p : null; }),
                     new KeyValuePair<Type, Func<string, object>>( typeof(byte?), x => { return byte.TryParse(x, out var p) ? (byte?) p : null; }),
                     new KeyValuePair<Type, Func<string, object>>( typeof(long?), x => { return long.TryParse(x, out var p) ? (long?) p : null; }),
                     new KeyValuePair<Type, Func<string, object>>( typeof(int?), x => { return int.TryParse(x, out var p) ? (int?) p : null; }),
                     new KeyValuePair<Type, Func<string, object>>( typeof(short?), x => { return short.TryParse(x, out var p) ? (short?) p : null; }),
                     new KeyValuePair<Type, Func<string, object>>( typeof(ushort?), x => { return ushort.TryParse(x, out var p) ? (ushort?) p : null; }),
                     new KeyValuePair<Type, Func<string, object>>( typeof(ulong?), x => { return ulong.TryParse(x, out var p) ? (ulong?) p : null; }),
                     new KeyValuePair<Type, Func<string, object>>( typeof(uint?), x => { return uint.TryParse(x, out var p) ? (uint?) p : null; }),
                     new KeyValuePair<Type, Func<string, object>>( typeof(float?), x => { return float.TryParse(x, out var p) ? (float?) p : null; }),
                     new KeyValuePair<Type, Func<string, object>>( typeof(double?), x => { return double.TryParse(x, out var p) ? (double?) p : null; }),
                     new KeyValuePair<Type, Func<string, object>>( typeof(decimal?), x => { return decimal.TryParse(x, out var p) ? (decimal?) p : null; }),
                     new KeyValuePair<Type, Func<string, object>>( typeof(DateTimeOffset?), x => { return DateTimeOffset.TryParse(x, out var p) ? (DateTimeOffset?) p : null; }),
                     new KeyValuePair<Type, Func<string, object>>( typeof(TimeSpan?), x => { return TimeSpan.TryParse(x, out var p) ? (TimeSpan?) p : null; }),
                }
            );

        public static object GetDefaultValue(this Type t)
        {
            if (t == typeof(string))
            {
                return default(string);
            }
            else
            {
                if (t.IsValueType)
                {
                    return Activator.CreateInstance(t);
                }
                else
                {
                    System.Reflection.ConstructorInfo cinfo = t.GetConstructors().Where(x => x.GetParameters().Count() == 0).FirstOrDefault();
                    if ((cinfo != null))
                    {
                        return Activator.CreateInstance(t);
                    }
                    else
                    {
                        throw new InvalidOperationException("No default constructor with no parameter is found for the type.");
                    }
                }
            }
        }


        /// <summary>
        /// Initializes static members of the <see cref="TypeExtensions"/> class.
        /// </summary>
        static TypeExtensions()
        {
            LoadPrimitiveTypeDefinition();
        }

        public static object ParseToTypedObject(object valueToParse, Type targetType)
        {
            if (valueToParse == null || targetType == typeof(object))
            {
                return valueToParse;
            }

            var filterValueType = valueToParse.GetType();
            object val = null;
            if (filterValueType == typeof(string))
            {
                var valAsString = (valueToParse as string).Trim();
                if (targetType == typeof(string) || !typeof(IEnumerable).IsAssignableFrom(targetType))
                {
                    if (valAsString.ValidateJSON())
                    {
                        val = JsonConvert.DeserializeObject(valAsString, targetType);
                    }
                    else
                    {
                        val = GetParserFunction(targetType)(valueToParse as string);
                    }
                }
                else
                {
                    if (targetType.IsGenericType)
                    {
                        var args = targetType.GenericTypeArguments;
                        if (args.Length == 1 && args.Single().IsPrimitive())
                        {
                            var itemType = args.Single();
                            if (itemType == typeof(string))
                            {
                                var strList = valAsString.Split(',').Select(x => x.Trim()).ToList();
                                val = (object) strList.ToList();
                            }
                            else
                            {
                                var filterStringList = valAsString
                                    .TrimStart('[').TrimEnd(']')
                                    .Split(',').Select(x => x.Trim()).ToList();

                                var parserFunc = GetParserFunction(itemType);
                                var typed = itemType.CreateTypedList();
                                filterStringList.ForEach(x => typed.Add(parserFunc(x)));
                                val = typed;
                            }
                        }
                        else if (args.Length == 2 && typeof(IDictionary).IsAssignableFrom(targetType))
                        {
                            var keyType = args.ElementAt(0);
                            var valueType = args.ElementAt(1);
                            var dictType = typeof(Dictionary<,>).MakeGenericType(keyType, valueType);
                            var value = JsonConvert.DeserializeObject(valAsString, dictType);
                            return value;
                        }
                        else
                        {
                            if (valAsString.StartsWith("[") && valAsString.EndsWith("]"))
                            {
                                return JsonConvert.DeserializeObject(valAsString, targetType);
                            }
                            else
                            {
                                throw new ArgumentException("Complex type array value string is expected to follow json formatting and it must begin and end with [ ]");
                            }
                        }
                    }
                    else
                    {
                        var filterStringList = (valueToParse as string).Split(',');
                        val = (object)filterStringList.ToList();
                    }

                }
            }
            else if (valueToParse is JToken)
            {
                var json = JsonConvert.SerializeObject(valueToParse);
                return ParseToTypedObject(json, targetType);
            }
            else if (ListElementTypesDiffer(valueToParse.GetType(), targetType, out var targetElementType))
            {
                var result = CreateListWithDifferentElementType(valueToParse, targetElementType);
                return result;
            }
            else
            {
                val = valueToParse;
            }

            return val;
        }

        public static TFilterValue ParseToTypedObject<TFilterValue>(object valueToParse)
        {
            var success = TryParseToTypedObject<TFilterValue>(valueToParse, out var result);
            if (success)
                return result;
            else
                throw new InvalidCastException(
                    $"An attempt was made to convert the value {valueToParse} to an instance of {typeof(TFilterValue).Name}");
        }

        public static bool TryParseToTypedObject<TFilterValue>(object valueToParse, out TFilterValue result)
        {
            var val = ParseToTypedObject(valueToParse, typeof(TFilterValue));

            if (val is TFilterValue typedValue)
            {
                result = typedValue;
                return true;
            }

            result = default;
            return false;
        }

        public static bool TryParseToTypedObject(object valueToParse, Type type, out object result)
        {
            var method = typeof(TypeExtensions)
                .GetMethods(BindingFlags.Public | BindingFlags.Static)
                .Where(m => m.Name == nameof(TypeExtensions.TryParseToTypedObject))
                .Single(m => m.IsGenericMethod);

            var genericMethod = method.MakeGenericMethod(type);
            var parameters = new object[] {valueToParse, null};
            var success = (bool) genericMethod.Invoke(null, parameters);
            result = parameters[1];
            return success;
        }

        public static TList CreateTypedList<TList>(this Type itemType)
        {
            var listType = typeof(List<>);
            var constructedListType = listType.MakeGenericType(itemType);
            var instance = Activator.CreateInstance(constructedListType);

            return (TList)instance;
        }

        public static IList CreateTypedList(this Type itemType)
        {
            var listType = typeof(List<>);
            var constructedListType = listType.MakeGenericType(itemType);
            var instance = Activator.CreateInstance(constructedListType);

            return (IList)instance;
        }

        public static object CreatedTypedDictionary(Type keyType, Type valueType)
        {
            var dictionaryType = typeof(Dictionary<,>);
            var constructedDictionaryType = dictionaryType.MakeGenericType(keyType, valueType);
            var instance = Activator.CreateInstance(constructedDictionaryType);

            return instance;
        }

        public static Func<string, object> GetParserFunction(Type typeLookup)
        {
            if (!parserLookup.ContainsKey(typeLookup))
            {
                if (typeLookup.IsEnum)
                {
                    return parserLookup.GetOrAdd(typeLookup, x =>
                    {
                        return Enum.Parse(typeLookup, x);
                    });
                }

                throw new NotSupportedException($"{ typeLookup.FullName } is not suppored by the type parser.");
            }

            return parserLookup[typeLookup];
        }

        private static bool ListElementTypesDiffer(Type sourceType, Type targetType, out Type targetElementType)
        {
            bool AssertGeneric(Type type) =>
                typeof(IEnumerable).IsAssignableFrom(type) &&
                type.IsGenericType &&
                type.GetGenericArguments().Length == 1;

            if (AssertGeneric(sourceType) && AssertGeneric(targetType))
            {
                targetElementType = targetType.GetGenericArguments()[0];
                var result = sourceType.GetGenericArguments()[0] != targetElementType;
                return result;
            }

            targetElementType = null;
            return false;
        }

        private static object CreateListWithDifferentElementType(object source, Type targetElementType)
        {
            IList result = CreateTypedList(targetElementType);
            var enumerator = (source as IEnumerable).GetEnumerator();
            while (enumerator.MoveNext())
            {
                var elem = enumerator.Current;
                var castElem = CastOrConvert(elem, targetElementType);
                result.Add(castElem);
            }

            return result;
        }

        public static bool IsCollectionType(this Type type) => IsCollectionType(type, out _);
        public static bool IsCollectionType(this Type type, out Type elementType)
        {
            bool IsGenericType(Type type, out List<Type> genericTypeArguments)
            {
                if (type.IsGenericType)
                {
                    genericTypeArguments = type.GetGenericArguments().ToList();
                    return true;
                }

                if (type.BaseType != typeof(object))
                {
                    return IsGenericType(type.BaseType, out genericTypeArguments);
                }
                else
                {
                    genericTypeArguments = new List<Type>();
                    return false;
                }
            }

            List<Type> genericArguments = null;

            var isGenericCollection =
                typeof(IEnumerable).IsAssignableFrom(type) &&
                IsGenericType(type, out genericArguments) &&
                genericArguments.Count == 1;

            if (isGenericCollection)
            {
                elementType = genericArguments.Single();
                var actualType = typeof(ICollection<>).MakeGenericType(elementType);
                var result = actualType.IsAssignableFrom(type);
                return result;
            }
            else
            {
                elementType = null;
                return false;
            }
        }

        /// <summary>
        /// Determines whether the this instance of type is primitive.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns><c>true</c> if the specified type is primitive; otherwise, <c>false</c>.</returns>
        public static bool IsPrimitive(this Type type)
        {
            if (_primitiveList.Any(x => x.IsAssignableFrom(type)))
                return true;

            var nut = Nullable.GetUnderlyingType(type);
            return nut != null && nut.IsEnum;
        }

        /// <summary>
        /// Gets the directly implemented interfaces on the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>IEnumerable&lt;Type&gt;.</returns>
        public static IEnumerable<Type> GetDirectlyImplementedInterfaces(this Type type)
        {
            var baseTypeFilteredInterfaces = type.GetInterfaces().Except(type.BaseType.GetInterfaces());
            return baseTypeFilteredInterfaces.Except(baseTypeFilteredInterfaces.SelectMany(x => x.GetInterfaces()).ToList());
        }

        private static void LoadPrimitiveTypeDefinition()
        {
            var types = new[]
            {
                typeof (Enum),
                typeof (String),
                typeof (Char),
                typeof (Guid),

                typeof (Boolean),
                typeof (Byte),
                typeof (Int16),
                typeof (Int32),
                typeof (Int64),
                typeof (Single),
                typeof (Double),
                typeof (Decimal),

                typeof (SByte),
                typeof (UInt16),
                typeof (UInt32),
                typeof (UInt64),

                typeof (DateTime),
                typeof (DateTimeOffset),
                typeof (TimeSpan),
            };


            var nullTypes = from t in types
                            where t.IsValueType
                            select typeof(Nullable<>).MakeGenericType(t);

            _primitiveList = types.Concat(nullTypes).ToList().AsReadOnly();
        }

        /// <summary>
        /// Parses the enum.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <returns>T.</returns>
        public static T ParseEnum<T>(this string value)
        {
            if (value == null)
                return default(T);

            return (T)Enum.Parse(typeof(T), value, true);
        }

        /// <summary>
        /// Converts the given <paramref name="value" /> to the desired type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static T ChangeType<T>(this object value)
        {
            if (value == null) return default(T);
            return (T)ChangeType(value, typeof(T));
        }

        /// <summary>
        /// Converts the given <paramref name="value" /> to the desired type.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="destinationType">Type of the destination.</param>
        /// <returns></returns>
        public static object ChangeType(this object value, Type destinationType)
        {
            try
            {
                // try a direct cast or TypeConverter
                var result = ConvertFrom(value, destinationType);
                if (result == null)
                    return Convert.ChangeType(value, destinationType);
                return result;
            }
            catch
            {
                // default to Convert, allow exceptions to blow up
                return Convert.ChangeType(value, destinationType);
            }
        }

        private static object ConvertFrom(object value, Type destinationType)
        {
            if (value != null && value.GetType() == destinationType)
            {
                return value;
            }

            var tc = TypeDescriptor.GetConverter(destinationType);
            if (tc.CanConvertFrom(value.GetType()))
            {
                return tc.ConvertFrom(value);
            }
            return null;
            
        }

        /// <summary>
        /// Creates the typed compare expression.
        /// NOTE: This is used to get around EF conversion issues with unspecified Type Parameters, e.g. Entity&lt;Tid&gt;.Id
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TKey">The type of the t key.</typeparam>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <returns>Expression&lt;Func&lt;T, System.Boolean&gt;&gt;.</returns>
        public static Expression<Func<T, bool>> CreateTypedCompareExpression<T, TKey>(string key, TKey value)
        {
            var bindingFlags = BindingFlags.Public | BindingFlags.Instance;

            var paramExpression = Expression.Parameter(typeof(T), "x");
            var propertyInfo = typeof(T).GetProperties(bindingFlags).FirstOrDefault(x => x.Name == key && x.PropertyType == typeof(TKey));
            var keyPropertyExpression = Expression.Property(paramExpression, propertyInfo);
            var constantExpression = Expression.Constant(value);

            var equalExpression = !IsNullableMember(keyPropertyExpression.Type)
                ? Expression.Equal(keyPropertyExpression, constantExpression)
                : NullableEquals(keyPropertyExpression, constantExpression);

            var conditionalExpression = Expression.Lambda<Func<T, bool>>(equalExpression, paramExpression);
            return conditionalExpression;
        }

        public static Expression<Func<T, bool>> CreateTypedCompareExpression<T>(string key, object value)
        {
            var bindingFlags = BindingFlags.Public | BindingFlags.Instance;

            var paramExpression = Expression.Parameter(typeof(T), "x");
            var propertyInfo = typeof(T).GetProperties(bindingFlags).FirstOrDefault(x => x.Name == key && x.PropertyType == value.GetType());
            var keyPropertyExpression = Expression.Property(paramExpression, propertyInfo);
            var constantExpression = Expression.Constant(value);

            var equalExpression = !IsNullableMember(keyPropertyExpression.Type)
                ? Expression.Equal(keyPropertyExpression, constantExpression)
                : NullableEquals(keyPropertyExpression, constantExpression);

            var conditionalExpression = Expression.Lambda<Func<T, bool>>(equalExpression, paramExpression);
            return conditionalExpression;
        }


        public static Expression<Func<T, bool>> AndAlso<T>(this Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2)
        {
            var parameter = Expression.Parameter(typeof(T), "x");

            var leftVisitor = new ReplaceExpressionVisitor(expr1.Parameters[0], parameter);
            var left = leftVisitor.Visit(expr1.Body);

            var rightVisitor = new ReplaceExpressionVisitor(expr2.Parameters[0], parameter);
            var right = rightVisitor.Visit(expr2.Body);

            return Expression.Lambda<Func<T, bool>>(
                Expression.AndAlso(left, right), parameter);
        }

        public static Expression<Func<T, bool>> Not<T>(this Expression<Func<T, bool>> expr1)
        {
            var parameter = Expression.Parameter(typeof(T), "x");

            var leftVisitor = new ReplaceExpressionVisitor(expr1.Parameters[0], parameter);
            var left = leftVisitor.Visit(expr1.Body);

            return Expression.Lambda<Func<T, bool>>(
                Expression.Not(left), parameter);
        }


        public static Expression<Func<T, bool>> OrElse<T>(this Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2)
        {
            var parameter = Expression.Parameter(typeof(T), "x");

            var leftVisitor = new ReplaceExpressionVisitor(expr1.Parameters[0], parameter);
            var left = leftVisitor.Visit(expr1.Body);

            var rightVisitor = new ReplaceExpressionVisitor(expr2.Parameters[0], parameter);
            var right = rightVisitor.Visit(expr2.Body);

            return Expression.Lambda<Func<T, bool>>(
                Expression.OrElse(left, right), parameter);
        }

        public static Expression<Func<T1, T2>> Use<T1, T2, T3, T4>(this Expression<Func<T3, T4>> expression,
                Expression<Func<T1, Func<T3, T4>, T2>> other)
        {
            return Expression.Lambda<Func<T1, T2>>(
                other.Body.Replace(other.Parameters[1], expression),
                other.Parameters[0]);
        }
        //another overload if there are two selectors
        public static Expression<Func<T1, T2>> Use<T1, T2, T3, T4, T5, T6>(
            this Expression<Func<T3, T4>> firstExpression,
            Expression<Func<T5, T6>> secondExpression,
            Expression<Func<T1, Func<T3, T4>, Func<T5, T6>, T2>> other)
        {
            return Expression.Lambda<Func<T1, T2>>(
                other.Body.Replace(other.Parameters[1], firstExpression)
                    .Replace(other.Parameters[2], secondExpression),
                other.Parameters[0]);
        }

        public static Expression Replace(this Expression expression, Expression searchEx, Expression replaceEx)
        {
            return new ReplaceExpressionVisitor(searchEx, replaceEx).Visit(expression);
        }

        public static bool IsNullableMember(this Type memberType)
        {
            return memberType.IsGenericType && memberType.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        internal static Expression NullableEquals(Expression memberExpression,
                                      ConstantExpression constantToCompare)
        {
            // Other cases removed, for simplicity. This answer only demonstrates
            // how to handle c => c.Weight != 5000f.
            var hasValueExpression = Expression.Property(memberExpression, "HasValue");
            var valueExpression = Expression.Property(memberExpression, "Value");
            var equals = Expression.Equal(valueExpression, constantToCompare);
            return Expression.AndAlso(hasValueExpression, equals);
        }

        public static Expression NullableComparision(
            Func<Expression, Expression, BinaryExpression> comparisionFunc,
            Expression memberExpression,
            ConstantExpression constantToCompare)
        {
            // Other cases removed, for simplicity. This answer only demonstrates
            // how to handle c => c.Weight != 5000f.
            var hasValueExpression = Expression.Property(memberExpression, "HasValue");
            var valueExpression = Expression.Property(memberExpression, "Value");
            var comparer = comparisionFunc(valueExpression, constantToCompare);
            return Expression.AndAlso(hasValueExpression, comparer);
        }

        public static BinaryExpression GetStringEvalOperationExpression(string stringOperationName, Expression a, Expression b)
        {
            MethodInfo method = typeof(string).GetMethod(stringOperationName, new[] { typeof(string) });
            var containsMethodExp = Expression.Call(a, method, b);
            var constExp = Expression.Constant(true);
            return Expression.Equal(containsMethodExp, constExp);
        }

        private class ReplaceExpressionVisitor
          : ExpressionVisitor
        {
            private readonly Expression _oldValue;
            private readonly Expression _newValue;

            public ReplaceExpressionVisitor(Expression oldValue, Expression newValue)
            {
                _oldValue = oldValue;
                _newValue = newValue;
            }

            public override Expression Visit(Expression node)
            {
                if (node == _oldValue)
                    return _newValue;
                return base.Visit(node);
            }
        }

        public static bool ValidateJSON(this string s)
        {
            try
            {
                //If it doesn't start with either one of these, it isn't json.
                if (!s.StartsWith("[") && !s.StartsWith("{")) return false;
                JToken.Parse(s);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static T CastOrConvertTo<T>(object value)
        {
            if (value == null)
                return default;

            if (TryParseToTypedObject<T>(value, out var castResult))
                return castResult;

            try
            {
                var result = value.ChangeType<T>();
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $"Could not cast or convert object of type '{value.GetType().Name}' to type '{typeof(T).Name}'.", ex);
            }


            //try
            //{
            //    var result = TypeExtensions.ParseToTypedObject<T>(value);
            //    return result;
            //}
            //catch //above method doesn't work from E.G. int to decimal - use conversion instead of cast
            //{
            //    try
            //    {
            //        var result = value.ChangeType<T>();
            //        return result;
            //    }
            //    catch (Exception ex)
            //    {
            //        throw new Exception(
            //            $"Could not cast or convert object of type '{value.GetType().Name}' to type '{typeof(T).Name}'.", ex);
            //    }
            //}
        }

        public static bool TryCastOrConvertTo<T>(object value, out T instance)
        {
            try
            {
                instance = CastOrConvertTo<T>(value);
                return true;
            }
            catch
            {
                instance = default;
                return false;
            }
        }

        public static object CastOrConvert(object value, Type type)
        {
            if (value == null)
                return null;
            try
            {
                var result = ChangeType(value, type);
                return result;
            }
            catch
            {
                try
                {
                    var result = ParseToTypedObject(value, type);
                    return result;
                }
                catch (Exception ex)
                {
                    throw new Exception(
                        $"Could not cast or convert object of type '{value.GetType().Name}' to type '{type.Name}'.", ex);
                }
            }
        }

        public static bool TryCastOrConvert(object value, Type type, out object instance)
        {
            try
            {
                instance = CastOrConvert(value, type);
                return true;
            }
            catch
            {
                instance = null;
                return false;
            }
        }

        public static IEnumerable<object> AsEnumerable(object value, Type elementType = null)
        {
            if (value != null && value is IEnumerable e)
            {
                var enumerator = e.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    if (elementType == null)
                        yield return enumerator.Current;
                    else
                        yield return CastOrConvert(enumerator.Current, elementType);
                }
            }
        }

        public static bool TryGetPropertyPath(
            string path, Type type,
            out IList<PropertyInfo> properties,
            Func<Type, string, PropertyInfo> accessProperty = null)
        {
            accessProperty ??= GetPropertyInfoByName;
            properties = new List<PropertyInfo>();
            var parts = path.Split('.').ToList();

            var i = 0;
            foreach (var part in parts)
            {
                var property = accessProperty.Invoke(type, part);
                if (property == null)
                {
                    if (i > 0) // they put the canonical object reference at the beginning "root.etc.etc", try again starting at index 1.
                        return false;
                    else
                        continue;
                }

                properties.Add(property);
                type = property.PropertyType;
            }

            return true;
        }

        private static PropertyInfo GetPropertyInfoByName(Type type, string name)
        {
            return type.GetProperty(name,
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase | BindingFlags.DeclaredOnly);
        }

        public static bool TryGetNestedPropertyValue(object root, string path, out object propertyValue)
        {
            propertyValue = null;
            var tmp = root;

            if (root == null)
                return false;

            if (TryGetPropertyPath(path, root.GetType(), out var properties))
            {
                foreach (var property in properties)
                {
                    if (property.GetMethod == null)
                        return false;

                    propertyValue = property.GetValue(tmp);
                    tmp = propertyValue;
                }
            }

            return true;
        }

        public static bool TrySetNestedPropertyValue(object root, string path, object propertyValue)
        {
            if (root == null)
                return false;

            var tmp = root;
            if (TryGetPropertyPath(path, root.GetType(), out var properties))
            {
                foreach (var prop in properties)
                {
                    if (prop != properties.Last()) // get to it
                    {
                        tmp = prop.GetValue(tmp);
                    }
                    else // then set it
                    {
                        if (prop.SetMethod == null)
                            return false;

                        try
                        {
                            prop.SetValue(tmp, propertyValue);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex);
                            //eat for now.
                        }
                    }
                }
            }

            return true;
        }

        /// <summary>
        ///     Convert a <see cref="JToken"/> and convert it to a .NET type (when you do not know the destination type).
        ///     If the passed object is not a <see cref="JToken"/>, it will just return the original object.
        ///     Useful when interacting with Trident Dyanmic Expressions accepting ad-hoc arguments.
        /// </summary>
        /// <param name="jsonValue"></param>
        /// <returns></returns>
        public static object ConvertJsonToPrimitiveTypeOrDictionary(object jsonValue)
        {
            if (jsonValue is JToken token)
            {
                switch (token.Type)
                {
                    case JTokenType.Object:
                        return ((JObject)token)
                            .Properties()
                            .ToDictionary(prop => prop.Name, prop => ConvertJsonToPrimitiveTypeOrDictionary(prop.Value));
                    case JTokenType.Array:
                        return CorrectGenericTypeArgumentInList(token.Values().Select(ConvertJsonToPrimitiveTypeOrDictionary).ToList());
                    case JTokenType.Integer:
                        return token.ToObject<int>();
                    case JTokenType.Float:
                        return token.ToObject<decimal>();
                    case JTokenType.String:
                    case JTokenType.Comment:
                    case JTokenType.Uri:
                        return token.ToObject<string>();
                    case JTokenType.Guid:
                        return token.ToObject<Guid>();
                    case JTokenType.Boolean:
                        return token.ToObject<bool>();
                    case JTokenType.TimeSpan:
                        return token.ToObject<TimeSpan>();
                    case JTokenType.Date:
                        return token.ToObject<DateTime>();
                    default:
                        return token.ToObject<object>();
                }
            }

            return jsonValue;
        }

        // take a List<object> and convert it to the appropriate element type (string, int, etc)
        private static object CorrectGenericTypeArgumentInList(object obj)
        {
            if (obj is List<object> list)
            {
                var types = list.Select(e => e.GetType()).Distinct().ToList();
                if (types.Count == 1)
                {
                    var genericType = types.Single();
                    var listType = typeof(List<>).MakeGenericType(genericType);
                    var newList = (IList)Activator.CreateInstance(listType);

                    var addMethod = listType.GetMethod(nameof(List<object>.Add));
                    foreach (var elem in list)
                    {
                        addMethod.Invoke(newList, new[] { elem });
                    }

                    return newList;
                }
            }

            return obj;
        }

        public static bool IsOfGenericBaseType(this Type type, Type genericType)
        {

            if (type == null || genericType == null || type == genericType)
                return false;

            if (genericType.IsGenericType == false && type.IsGenericType == false)
            {
                return type.IsSubclassOf(genericType);
            }
            else
            {
                genericType = genericType.GetGenericTypeDefinition();
            }

            Type objectType = typeof(object);

            while (type != objectType && type != null)
            {
                Type curentType = type.IsGenericType ?
                    type.GetGenericTypeDefinition() : type;
                if (curentType == genericType)
                    return true;

                type = type.BaseType;
            }

            return false;
        }

        /// <summary>
        ///     Compare the values of common key value pairs shared between two dictionaries.
        /// </summary>
        /// <typeparam name="TKey">Key type.</typeparam>
        /// <typeparam name="TValue">Value type.</typeparam>
        /// <param name="dict1">First dictionary in comparison.</param>
        /// <param name="dict2">Second dictionary in comparison.</param>
        /// <param name="keyComparer">Key comparer</param>
        /// <param name="valueComparer">Value comparer</param>
        /// <returns><see langword="true"/> if the common key values are equal in both dictionaries. Otherwise, <see langword="false"/>.</returns>
        public static bool CompareCommonKeyValuePairs<TKey, TValue>(
            IDictionary<TKey, TValue> dict1,
            IDictionary<TKey, TValue> dict2,
            IEqualityComparer<TKey> keyComparer = null,
            IEqualityComparer<TValue> valueComparer = null)
        {
            dict1.GuardIsNotNull(nameof(dict1));
            dict2.GuardIsNotNull(nameof(dict2));
            keyComparer ??= EqualityComparer<TKey>.Default;
            valueComparer ??= EqualityComparer<TValue>.Default;
            var commonKeys = dict1.Keys.Intersect(dict2.Keys, keyComparer);

            foreach (var key in commonKeys)
            {
                var value1 = dict1[key];
                var value2 = dict2[key];

                if (!valueComparer.Equals(value1, value2))
                    return false;
            }

            return true;
        }

        /// <summary>
        ///     Hydrate properties in the target object that are null with the [NotMapped] attribute.
        /// <para>
        ///     The source and target object must both be of the same type.
        /// </para>
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        public static void HydrateNullNotMappedProperties(object source, object target) // TODO: Remove
        {
            if (source != null && target != null)
            {
                var properties = source.GetType()
                    .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(prop => prop.GetCustomAttribute<NotMappedAttribute>() != null);

                foreach (var property in properties)
                {
                    var targetCurrentValue = property.GetValue(target);
                    var sourceCurrentValue = property.GetValue(source);

                    if (targetCurrentValue == null && sourceCurrentValue != null)
                    {
                        property.SetValue(target, sourceCurrentValue);
                    }
                }
            }
        }


        public static string GetFriendlyName(this Type type)
        {
            string friendlyName = type.Name;
            if (type.IsGenericType)
            {
                int iBacktick = friendlyName.IndexOf('`');
                if (iBacktick > 0)
                {
                    friendlyName = friendlyName.Remove(iBacktick);
                }
                friendlyName += "<";
                Type[] typeParameters = type.GetGenericArguments();
                for (int i = 0; i < typeParameters.Length; ++i)
                {
                    string typeParamName = GetFriendlyName(typeParameters[i]);
                    friendlyName += (i == 0 ? typeParamName : "," + typeParamName);
                }
                friendlyName += ">";
            }

            return friendlyName;
        }
    }
}
