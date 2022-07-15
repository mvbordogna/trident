using System.Reflection;
using Trident.Business;
using Trident.Caching;
using Trident.Common;
using Trident.Contracts;
using Trident.Contracts.Configuration;
using Trident.Data;
using Trident.Data.Contracts;
using Trident.Rest;
using Trident.Rest.Contracts;
using Trident.Search;
using Trident.Transactions;
using Trident.Validation;
using Trident.Workflow;

namespace Trident.IoC
{
    public static class TridentIocExtensions
    {

        public static IIoCProvider UsingTridentEntityComparer(this IIoCProvider builder)
        {
            return builder.Register<EntityComparer, IEntityComparer>(LifeSpan.SingleInstance);
        }

        public static IIoCProvider UsingTridentFileStorage(this IIoCProvider builder)
        {
            builder.Register<DefaultFileStorageProvider, IFileStorageProvider>(LifeSpan.SingleInstance);
            return builder.Register<DefaultFileStorageManager, IFileStorageManager>(LifeSpan.SingleInstance);

        }

        public static IIoCProvider UsingTridentData(this IIoCProvider builder)
        {
            builder.Register<AbstractContextFactory, IAbstractContextFactory>(LifeSpan.InstancePerLifetimeScope);
            builder.Register<SharedConnectionStringResolver, ISharedConnectionStringResolver>(LifeSpan.SingleInstance);
            builder.Register<RestAuthenticationProviderFactory, IRestAuthenticationFactory>(LifeSpan.SingleInstance);
            builder.Register<RestAuthenticationProviderFactory, IRestAuthenticationFactory>(LifeSpan.SingleInstance);
            builder.Register<RestConnectionStringResolver, IRestConnectionStringResolver>(LifeSpan.SingleInstance);
            return builder;
        }



        public static IIoCProvider UsingTridentAppSettingsXmlManager(this IIoCProvider builder)

        {
            return builder.RegisterSingleton<Common.XmlAppSettings, IAppSettings>(true);         
        }

        public static IIoCProvider UsingTridentAppSettingsJsonManager(this IIoCProvider builder)
        {
            return builder.RegisterSingleton<Common.JsonAppSettings, IAppSettings>(true);
        }

        public static IIoCProvider UsingTridentConnectionStringXmlManager(this IIoCProvider builder)
        {
            return builder.RegisterSingleton<XmlConnectionStringSettings, IConnectionStringSettings>(true);            
        }

        public static IIoCProvider UsingTridentConnectionStringJsonManager(this IIoCProvider builder)
        {
            return builder.RegisterSingleton<JsonConnectionStringSettings, IConnectionStringSettings>(true);
        }


        /// <summary>
        /// Registers the data source dependencies.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="targetAssemblies">The target assemblies.</param>
        public static void UsingTridentSearch(this IIoCProvider builder, params Assembly[] targetAssemblies)
        {
            builder.UsingTridentSearchResultBuilder();
            builder.UsingTridentSearchQueryBuilder();
            builder.UsingTridentSearchResultBuilder();
            builder.UsingTridentSearchComplexFilterFactory();
            builder.UsingTridentComplexFilters(targetAssemblies);
            builder.UsingTridentComplexFilterAdapters(targetAssemblies);
        }

        public static IIoCProvider UsingTridentTransactions(this IIoCProvider builder)
        {
            return builder.Register<TransactionScopeFactory, ITransactionScopeFactory>(LifeSpan.SingleInstance);
        }

        public static IIoCProvider UsingTridentInMemberCachingManager(this IIoCProvider builder)
        {
            return builder.Register<InMemoryCachingManager, ICachingManager>(LifeSpan.SingleInstance);
        }

        public static IIoCProvider UsingTridentValidationManagers(this IIoCProvider builder, params Assembly[] targetAssemblies)

        {
            builder.RegisterGeneric(typeof(DefaultValidationManager<>), typeof(IValidationManager<>), targetAssemblies, true, LifeSpan.InstancePerLifetimeScope);
            return builder.RegisterAll(typeof(IValidationManager), targetAssemblies, true, LifeSpan.InstancePerLifetimeScope);
        }

        public static IIoCProvider UsingTridentValidationRules(this IIoCProvider builder, params Assembly[] targetAssemblies)
        {
            return builder.RegisterAll(typeof(IValidationRule), targetAssemblies, true, LifeSpan.InstancePerLifetimeScope);

        }

        public static IIoCProvider UsingTridentWorkflowManagers(this IIoCProvider builder, params Assembly[] targetAssemblies)

        {
            //this registers the default
            builder.RegisterGeneric(typeof(DefaultWorkflowManager<>), typeof(IWorkflowManager<>), targetAssemblies, true, LifeSpan.InstancePerLifetimeScope);
            return builder.RegisterAll(typeof(IWorkflowManager), targetAssemblies, true, LifeSpan.InstancePerLifetimeScope);

        }

        public static IIoCProvider UsingTridentWorkflowTasks(this IIoCProvider builder, params Assembly[] targetAssemblies)

        {
            return builder.RegisterAll(typeof(IWorkflowTask), targetAssemblies, true, LifeSpan.InstancePerLifetimeScope);
        }

        public static IIoCProvider UsingTridentSearchResultBuilder(this IIoCProvider builder)
        {
            return builder.Register<SearchResultsBuilder, ISearchResultsBuilder>();
        }

        public static IIoCProvider UsingTridentSearchQueryBuilder(this IIoCProvider builder)
        {
            return builder.Register<SearchQueryBuilder, ISearchQueryBuilder>();
        }

        public static IIoCProvider UsingTridentSearchComplexFilterFactory(this IIoCProvider builder)
        {
            return builder.Register<ComplexFilterFactory, IComplexFilterFactory>();
        }

        public static IIoCProvider UsingTridentComplexFilters(this IIoCProvider builder, params Assembly[] targetAssemblies)
        {
            return builder.RegisterAll<IComplexFilter>(targetAssemblies, LifeSpan.InstancePerLifetimeScope);
        }

        public static IIoCProvider UsingTridentComplexFilterAdapters(this IIoCProvider builder, params Assembly[] targetAssemblies)
        {
            return builder.RegisterAll<IComplexFilterAdapter>(targetAssemblies, LifeSpan.InstancePerLifetimeScope);
        }
    }
}
