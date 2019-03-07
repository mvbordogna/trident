using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

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

                     new KeyValuePair<Type, Func<string, object>>( typeof(bool?), x => { return bool.Parse(x); }),
                     new KeyValuePair<Type, Func<string, object>>( typeof(Guid?), x => { return Guid.Parse(x); }),
                     new KeyValuePair<Type, Func<string, object>>( typeof(DateTime?), x => { return DateTime.Parse(x); }),
                     new KeyValuePair<Type, Func<string, object>>( typeof(char?), x => { return char.Parse(x);    }),
                     new KeyValuePair<Type, Func<string, object>>( typeof(SByte?), x => { return SByte.Parse(x);}),
                     new KeyValuePair<Type, Func<string, object>>( typeof(byte?), x => { return byte.Parse(x);}),
                     new KeyValuePair<Type, Func<string, object>>( typeof(long?), x => { return long.Parse(x); }),
                     new KeyValuePair<Type, Func<string, object>>( typeof(int?), x => { return int.Parse(x); }),
                     new KeyValuePair<Type, Func<string, object>>( typeof(short?), x => { return short.Parse(x); }),
                     new KeyValuePair<Type, Func<string, object>>( typeof(ushort?), x => { return ushort.Parse(x); }),
                     new KeyValuePair<Type, Func<string, object>>( typeof(ulong?), x => { return ulong.Parse(x); }),
                     new KeyValuePair<Type, Func<string, object>>( typeof(uint?), x => { return uint.Parse(x); }),
                     new KeyValuePair<Type, Func<string, object>>( typeof(float?), x => { return float.Parse(x); }),
                     new KeyValuePair<Type, Func<string, object>>( typeof(double?), x => { return double.Parse(x); }),
                     new KeyValuePair<Type, Func<string, object>>( typeof(decimal?), x => { return decimal.Parse(x); }),
                     new KeyValuePair<Type, Func<string, object>>( typeof(DateTimeOffset?), x => { return DateTimeOffset.Parse(x); }),
                     new KeyValuePair<Type, Func<string, object>>( typeof(TimeSpan?), x => { return TimeSpan.Parse(x); }),
                }
            );




        /// <summary>
        /// Initializes static members of the <see cref="TypeExtensions"/> class.
        /// </summary>
        static TypeExtensions()
        {
            LoadPrimitiveTypeDefinition();
        }

        public static object ParseToTypedObject(object valueToParse, Type targetType)
        {
            var filterValueType = valueToParse.GetType();
            object val = null;
            if (filterValueType == typeof(string))
            {
                if (targetType == typeof(string) || !typeof(IEnumerable).IsAssignableFrom(targetType))
                {
                    val = GetParserFunction(targetType)(valueToParse as string);
                }
                else
                {
                    if (targetType.IsGenericType)
                    {
                        var itemType = targetType.GenericTypeArguments.First();
                        var valAsString = (valueToParse as string).Trim();
                        List<string> filterStringList = null;

                        if (itemType.IsPrimitive())
                        {
                            filterStringList = valAsString.Split(',').Select(x => x.Trim()).ToList();

                            if (itemType == typeof(string))
                            {
                                val = (object)filterStringList.ToList();
                            }
                            else
                            {
                                var parserFunc = GetParserFunction(itemType);
                                var typed = itemType.CreateTypedList();
                                filterStringList.ForEach(x => typed.Add(parserFunc(x)));
                                val = typed;
                            }
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
            else
            {
                val = valueToParse;
            }

            return val;
        }

        public static TFilterValue ParseToTypedObject<TFilterValue>(object valueToParse)
        { 
           var val =  ParseToTypedObject(valueToParse, typeof(TFilterValue));

            TFilterValue filterVal = val != null
                ? (TFilterValue)val
                : default(TFilterValue);

            return filterVal;
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
                return ConvertFrom(value, destinationType);
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
            return tc.ConvertFrom(value);
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

            var paramExpression = Expression.Parameter(typeof(T), key);
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

            var paramExpression = Expression.Parameter(typeof(T), "entity");
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
            var parameter = Expression.Parameter(typeof(T));

            var leftVisitor = new ReplaceExpressionVisitor(expr1.Parameters[0], parameter);
            var left = leftVisitor.Visit(expr1.Body);

            var rightVisitor = new ReplaceExpressionVisitor(expr2.Parameters[0], parameter);
            var right = rightVisitor.Visit(expr2.Body);

            return Expression.Lambda<Func<T, bool>>(
                Expression.AndAlso(left, right), parameter);
        }


        public static Expression<Func<T, bool>> OrElse<T>(this Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2)
        {
            var parameter = Expression.Parameter(typeof(T));

            var leftVisitor = new ReplaceExpressionVisitor(expr1.Parameters[0], parameter);
            var left = leftVisitor.Visit(expr1.Body);

            var rightVisitor = new ReplaceExpressionVisitor(expr2.Parameters[0], parameter);
            var right = rightVisitor.Visit(expr2.Body);

            return Expression.Lambda<Func<T, bool>>(
                Expression.OrElse(left, right), parameter);
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

    }
}




