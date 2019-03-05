using AutoMapper;
using System;
using System.Linq;
using System.Data;
using Trident.Extensions;
using System.Collections.Generic;
using System.Linq.Expressions;
using Trident.Validation;
using Trident.Domain;

namespace Trident.Mapper
{
    /// <summary>
    /// Provides an interface over AutoMapper so that mapping can be mocked,
    /// and AutoMapper is not directly coupled to application source code.
    /// </summary>
    public abstract class AutoMapperRegistry : IMapperRegistry
    {
        private readonly IMapper _mappingEngine;
        private readonly MapperConfiguration _configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="AutoMapperRegistry" /> class.
        /// </summary>
        protected AutoMapperRegistry() : this(new MapperConfiguration(cfg => { }))
        {
        }        

        /// <summary>
        /// Initializes a new instance of the <see cref="AutoMapperRegistry" /> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        protected AutoMapperRegistry(MapperConfiguration configuration)
        {
            try
            {
                _configuration = configuration;
                this._mappingEngine = configuration.CreateMapper();
              
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        /// <summary>
        /// Gets the mapping engine.
        /// </summary>
        /// <value>The mapping engine.</value>
        protected IMapper MappingEngine { get { return _mappingEngine; } }

        /// <summary>
        /// Maps the specified source.
        /// </summary>
        /// <typeparam name="TDestination">The type of the t destination.</typeparam>
        /// <param name="source">The source.</param>
        /// <returns>TDestination.</returns>
        /// <exception cref="MappingException"></exception>      
        public TDestination Map<TDestination>(object source) where TDestination : class
        {
            source.GuardIsNotNull("source");

            try
            {
                return _mappingEngine.Map<TDestination>(source);
            }
            catch (Exception exception)
            {
                var exToThrow = HandleIReaderSpecialExceptions<TDestination>(exception, source.GetType());
                if (exToThrow == null)
                    throw new MappingException(source.GetType(), typeof(TDestination), exception);

                throw exToThrow;
            }
        }

        /// <summary>
        /// Provides a method to examine less obvous exceptions generated during an IDataReader mapping operation.
        /// </summary>
        /// <typeparam name="TDestination">The type of the t destination.</typeparam>
        /// <param name="exception">The exception.</param>
        /// <param name="sourceType">Type of the source.</param>
        /// <returns>MappingException.</returns>
        private MappingException HandleIReaderSpecialExceptions<TDestination>(Exception exception, Type sourceType)
        {
            if (typeof(IDataReader).IsAssignableFrom(sourceType))
            {
                var tempEx = exception;

                while (tempEx.InnerException != null)
                {
                    tempEx = tempEx.InnerException;
                }
                var dest = typeof(TDestination);
                var genericType = dest.GenericTypeArguments.FirstOrDefault();
                var typeToDisplay = genericType ?? dest;

                if (tempEx.Message.ToLower().Contains("cast"))
                {
                    var informMsg = string.Format("\n *** The Most Likely cause is a type mismatch or type conversion is not implicitly possible "
                        + "between a column and Property match of {0}.",
                        typeToDisplay.FullName);
                    return new MappingException("An IDataReader mapping error occurred: " + tempEx.Message + informMsg, exception);
                }
                else if (tempEx.Message.ToLower().Contains("datarecordinternal"))
                {
                    var informMsg = string.Format("\n *** mapperRegistry.Map<>() function Requires that both TSource and TDestination are specified when mapping "
                        + "with an DataReader. Add the IDataReader Interface type to the first generic parameter of the Map Method call for the Map to {0}.",
                        typeToDisplay.FullName);
                    return new MappingException("An IDataReader mapping error occurred: " + tempEx.Message + informMsg, exception);
                }
                else if (tempEx is IndexOutOfRangeException)
                {
                    var informMsg = string.Format("\n *** The Most Likely cause is a Property Member name or alias \"{1}\" of Type {0} was not found in "
                        + "the result set. If this is intended, add a .ForMember Ignore to the Auto Mapping configuration.",

                        typeToDisplay.FullName, tempEx.Message);
                    return new MappingException("An IDataReader mapping error occurred: " + tempEx.Message + informMsg, exception);
                }

            }

            return null;
        }


        /// <summary>
        /// Maps the specified source.
        /// </summary>
        /// <typeparam name="TSource">The type of the t source.</typeparam>
        /// <typeparam name="TDestination">The type of the t destination.</typeparam>
        /// <param name="source">The source.</param>
        /// <returns>TDestination.</returns>
        /// <exception cref="MappingException"></exception>
        public TDestination Map<TSource, TDestination>(TSource source)
        {
            source.GuardIsNotNull("source");
            try
            {
                return _mappingEngine.Map<TSource, TDestination>(source);
            }
            catch (System.Exception exception)
            {
                var exToThrow = HandleIReaderSpecialExceptions<TDestination>(exception, source.GetType());
                if (exToThrow == null)
                    throw new MappingException(typeof(TSource), typeof(TDestination), exception);

                throw exToThrow;
            }
        }

        /// <summary>
        /// Maps the specified source.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <typeparam name="TDestination">The type of the destination.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="destination">The destination.</param>
        /// <exception cref="MappingException"></exception>       
        public void Map<TSource, TDestination>(TSource source, TDestination destination)
            where TSource : class
            where TDestination : class
        {
            source.GuardIsNotNull("source");

            try
            {
                _mappingEngine.Map(source, destination);
            }
            catch (System.Exception exception)
            {
                var exToThrow = HandleIReaderSpecialExceptions<TDestination>(exception, source.GetType());
                if (exToThrow == null)
                    throw new MappingException(source.GetType(), typeof(TDestination), exception);

                throw exToThrow;
            }
        }

        /// <summary>
        /// Asserts the configuration is valid.
        /// </summary>
        public void AssertConfigurationIsValid()
        {
            _configuration.AssertConfigurationIsValid();
        }

        /// <summary>
        /// Dynamics the map.
        /// </summary>
        /// <typeparam name="TSource">The type of the t source.</typeparam>
        /// <typeparam name="TDestination">The type of the t destination.</typeparam>
        /// <param name="source">The source.</param>
        /// <returns>TDestination.</returns>
        public TDestination DynamicMap<TSource, TDestination>(TSource source)
        {
            return this._mappingEngine.Map<TSource, TDestination>(source);
        }



        public void MapPropertyExpressions<TErrorCodes, TEntity, TDto>(IEnumerable<Validation.ValidationResult> validationResults)
              where TErrorCodes : struct, IConvertible
             where TEntity : Entity
        {
            var propertyMaps = this._mappingEngine.ConfigurationProvider.ResolveTypeMap(typeof(TEntity), typeof(TDto)).PropertyMaps;
            var propertyDict = propertyMaps.Select(x => new
            {
                ExpressionLookup = (x.CustomMapExpression != null && x.SourceMember != null)
                    ? GetBareExpressionString(x.CustomMapExpression)
                    : null,
                DirectMemberLookup = x.SourceMember?.Name,
                PropertyMap = x
            })
            .Where(x => (x.ExpressionLookup ?? x.DirectMemberLookup) != null)
            .ToDictionary(x => x.ExpressionLookup ?? x.DirectMemberLookup, x => x.PropertyMap);


            foreach (var result in validationResults)
            {
                var genericResult = result as ValidationResult<TErrorCodes, TEntity>;
                if (genericResult != null && genericResult.MemberExpressions.Any())
                {
                    foreach (var expression in genericResult.MemberExpressions)
                    {
                        var bareExpression = GetBareExpressionString(expression);

                        if (propertyDict.ContainsKey(bareExpression))
                        {
                            genericResult.AddMemberName(propertyDict[bareExpression].DestinationMember.Name);
                        }
                        else
                        {
                            genericResult.ApplyExpression(expression);
                        }
                    }
                }
            }
        }

        private static string GetBareExpressionString(LambdaExpression expression)
        {
            var startPos = expression.Parameters[0].ToString().Length + 1;

            MemberExpression memberExpression = null;
            if (expression.Body is ConditionalExpression)
            {
                var temp = expression.Body as ConditionalExpression;
                memberExpression = temp.IfTrue as MemberExpression ?? temp.IfFalse as MemberExpression;
            }
            else
            {
                memberExpression = (expression.Body as MemberExpression
                 ?? ((UnaryExpression)expression.Body).Operand as MemberExpression);
            }

            while (memberExpression.NodeType != ExpressionType.MemberAccess)
            {
                memberExpression = (memberExpression.Expression as MemberExpression
                ?? ((UnaryExpression)memberExpression.Expression).Operand as MemberExpression);
            }
            return memberExpression.ToString().Substring(startPos);
        }
    }
}
