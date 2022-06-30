using AutoMapper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Trident.Api.Search;
using Trident.Domain;
using Trident.IoC;
using Trident.Search;

namespace Trident.Mapper
{
    /// <summary>
    /// Adds Extension methods to the public accessible types for AutoMapper.
    /// </summary>
    public static class AutoMapperExtensions
    {

        public static IIoCProvider UsingTridentMapperProfiles(this IIoCProvider builder, params Assembly[] targetAssemblies)
        {
            builder.Register<ServiceMapperRegistry, IMapperRegistry>(LifeSpan.SingleInstance);
            return builder.RegisterAllAsTarget<AutoMapper.Profile>(targetAssemblies, LifeSpan.SingleInstance);

        }


        /// <summary>
        /// Use this method before any forMember invocation mapping definions It will clear all mappings,
        /// NOTE: Property names that match between two types will automatically have a map create for them,
        /// this is a way to clear all mappings, the only add the ones you want.
        /// if use
        /// </summary>
        /// <typeparam name="TSource">The type of the t source.</typeparam>
        /// <typeparam name="TDest">The type of the t dest.</typeparam>
        /// <param name="expression">The expression.</param>
        /// <returns>IMappingExpression&lt;TSource, TDest&gt;.</returns>
        public static IMappingExpression<TSource, TDest> ClearAllMappings<TSource, TDest>(this IMappingExpression<TSource, TDest> expression)
        {
            expression.ForAllMembers(opt => opt.Ignore());
            return expression;
        }

        /// <summary>
        /// Ignores all non existing.
        /// This is used to support legacy mappings that were not explicitly ignoring unmatched properies.
        /// NOTE: **this should not be used for new code
        /// </summary>
        /// <typeparam name="TSource">The type of the t source.</typeparam>
        /// <typeparam name="TDestination">The type of the t destination.</typeparam>
        /// <param name="expression">The expression.</param>
        /// <returns>IMappingExpression&lt;TSource, TDestination&gt;.</returns>
        public static IMappingExpression<TSource, TDestination> IgnoreAllNonExisting<TSource, TDestination>(this IMappingExpression<TSource, TDestination> expression)
        {
            var flags = BindingFlags.Public | BindingFlags.Instance;
            var sourceProperties = new HashSet<string>(typeof(TSource).GetProperties(flags)
                .Select(x => x.Name.ToLower()).Distinct());
            var destinationProperties = typeof(TDestination).GetProperties(flags);

            foreach (var property in destinationProperties)
            {
                if (!sourceProperties.Contains(property.Name.ToLower()))
                {
                    expression.ForMember(property.Name, opt => opt.Ignore());
                }
            }
            return expression;
        }

        public static void ConfigureAllSupportedPrimitiveTypes(this Profile cfg)
        {
            Type[] supportedTypes =
                {
                typeof(Guid), typeof(string), typeof(int),
                typeof(long), typeof(decimal), typeof(DateTime),
                typeof(DateTimeOffset), typeof(float), typeof(double)
            };
            var openGenericConfigMethod = typeof(AutoMapperExtensions)
                .GetMethod(nameof(ConfigurePrimiativeCollectionMapping), BindingFlags.NonPublic | BindingFlags.Static);
            var parameters = new object[] { cfg };
            foreach (Type type in supportedTypes)
            {
                openGenericConfigMethod.MakeGenericMethod(type)
                    .Invoke(null, parameters);
            }
        }

        private static void ConfigurePrimiativeCollectionMapping<T>(Profile cfg)
        {
            cfg.CreateMap<List<T>, PrimitiveCollection<T>>()

               .ForMember(x => x.Key, x => x.Ignore())
               .ForMember(x => x.Raw, x => x.Ignore())
               .ForMember(x => x.List, x => x.MapFrom(y => y));

            cfg.CreateMap<PrimitiveCollection<T>, List<T>>()
                .AfterMap((s, d) =>
                {
                    d.AddRange(s.List);
                });
        }


        public static void ConfigureSearchMapping(this IProfileExpression profile)
        {
            profile.CreateMap<SearchCriteriaModel, SearchCriteria>().ReverseMap();

            profile.CreateMap(typeof(SearchCriteriaModel), typeof(SearchCriteria<>))
                .ForMember(nameof(SearchCriteria.detach), cfg => cfg.Ignore())
                .ForMember("CustomFilter", cfg => cfg.Ignore())
                .ForMember(nameof(SearchCriteria.ContextBag), cfg => cfg.Ignore())
                .ForMember(nameof(SearchCriteria.ApplyDefaultFilters), cfg => cfg.Ignore())
                .ForMember(nameof(SearchCriteria.Filters), cfg => cfg.Ignore())
                .AddAdvancedFilterTypeMapping();
            ;
            profile.CreateMap(typeof(SearchCriteria<>), typeof(SearchCriteriaModel));
            profile.CreateMap(typeof(SearchResultInfo<>), typeof(SearchResultInfoModel<>));
            profile.CreateMap(typeof(SearchResultInfoModel<>), typeof(SearchResultInfo<>));
            profile.CreateMap(typeof(SearchResultsModel<,>), typeof(SearchResults<,>));
            profile.CreateMap(typeof(SearchResults<,>), typeof(SearchResultsModel<,>));
            profile.CreateMap<CompareModel, Compare>().ReverseMap();
            profile.CreateMap<AxiomFilterModel, AxiomFilter>().ReverseMap();
            profile.CreateMap<AxiomModel, Axiom>().ReverseMap();

            //profile.CreateMap(typeof(SearchCriteriaModel), typeof(LookupSearchCriteria<>))
            //    .ForMember(nameof(LookupSearchCriteria.IdMember), cfg => cfg.Ignore())
            //    .ForMember(nameof(LookupSearchCriteria.DisplayMember), cfg => cfg.Ignore())
            //    .ForMember(nameof(LookupSearchCriteria.detach), cfg => cfg.Ignore())
            //    ;

            //profile.CreateMap(typeof(LookupSearchResults<,>), typeof(SearchResultsModel<,>))
            //    ;

            //profile.CreateMap(typeof(LookupSearchResultInfo<>), typeof(SearchResultInfoModel<>))
            //    ;

            //profile.CreateMap<Core.Domain.Lookup, LookupModel>().ReverseMap();




        }
        public static void ConfigureAllSupportedPrimitiveCollectionTypes(this Profile cfg)
        {
            Type[] supportedTypes =
                {
                typeof(Guid), typeof(string), typeof(int),
                typeof(long), typeof(decimal), typeof(DateTime),
                typeof(DateTimeOffset), typeof(float), typeof(double)
            };
            var openGenericConfigMethod = typeof(AutoMapperExtensions)
                .GetMethod(nameof(ConfigurePrimiativeCollectionMapping), BindingFlags.NonPublic | BindingFlags.Static);
            var parameters = new object[] { cfg };
            foreach (Type type in supportedTypes)
            {
                openGenericConfigMethod.MakeGenericMethod(type)
                    .Invoke(null, parameters);
            }
        }
        public static IMappingExpression AddAdvancedFilterTypeMapping(this IMappingExpression targetmap)
        {
            return targetmap
                .ForMember(nameof(SearchCriteria.Filters), cfg => cfg.Ignore())
                .AfterMap((s, d) => MapFiltersToEntity(s as SearchCriteriaModel, d as SearchCriteria));
        }

        private static void MapFiltersToEntity(SearchCriteriaModel model, SearchCriteria entity)
        {
            entity.Filters.Clear();
            var setting = new JsonSerializerSettings()
            {

            };
            foreach (var filter in model.Filters)
            {
                if (filter.Value != null)
                {
                    var filterValueType = filter.Value.GetType();



                    if (filterValueType.IsPrimitive())
                    {
                        entity.Filters.Add(filter.Key, filter.Value);
                    }
                    else if (filter.Value is AxiomFilterModel)
                    {
                        var axiomfilter = JsonConvert.DeserializeObject<AxiomFilter>(JsonConvert.SerializeObject(filter.Value), setting);
                        entity.AddFilter(filter.Key, axiomfilter);
                    }
                    else if (filter.Value is AxiomModel)
                    {
                        var axiom = JsonConvert.DeserializeObject<Axiom>(JsonConvert.SerializeObject(filter.Value), setting);
                        entity.AddFilter(filter.Key, axiom);
                    }
                    else if (filter.Value is CompareModel)
                    {
                        var compare = JsonConvert.DeserializeObject<Compare>(JsonConvert.SerializeObject(filter.Value), setting);
                        entity.AddFilter(filter.Key, compare);
                    }
                    else
                    {
                        var clone = JsonConvert.DeserializeObject(JsonConvert.SerializeObject(filter.Value), filterValueType, setting);

                        entity.AddFilter(filter.Key, clone);
                    }
                }
            }
        }

    }
}
