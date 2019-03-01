namespace Trident
{
    using System.Collections.Generic;
    using Business;
    using Common;
    using Contracts;
    using Data;
    using Data.Contracts;
    using IoC;
    using Rest;
    using Rest.Contracts;
    using Search;
    using Transactions;

    public class RegistrationModule : IoC.TypeRegistrationModule
    {
        public override List<ITypeRegistration> GetRegistrations()
        {
            return new List<ITypeRegistration>() {
                new TypeRegistration<SearchResultsBuilder, ISearchResultsBuilder>(LifeSpan.SingleInstance),
                new TypeRegistration<SearchQueryBuilder, ISearchQueryBuilder>(LifeSpan.SingleInstance),
                new TypeRegistration<ComplexFilterFactory, IComplexFilterFactory>(LifeSpan.SingleInstance),
                new TypeRegistration<AbstractContextFactory, IAbstractContextFactory>(LifeSpan.InstancePerLifetimeScope),
                new TypeRegistration<XmlAppSettings, IAppSettings>(LifeSpan.SingleInstance),
                new TypeRegistration<TransactionScopeFactory, ITransactionScopeFactory>(LifeSpan.SingleInstance),
                new TypeRegistration<RestAuthenticationProviderFactory, IRestAuthenticationFactory>(LifeSpan.SingleInstance),
                new TypeRegistration<RestAuthenticationProviderFactory, IRestAuthenticationFactory>(LifeSpan.SingleInstance),
                new TypeRegistration<RestConnectionStringResolver, IRestConnectionStringResolver>(LifeSpan.SingleInstance),
                new TypeRegistration<DefaultFileStorageProvider, IFileStorageProvider>(LifeSpan.SingleInstance),
                new TypeRegistration<DefaultFileStorageManager, IFileStorageManager>(LifeSpan.SingleInstance),
            };
        }

        public override List<ITypeRegistration> GetScanRegistrations()
        {
            return new List<ITypeRegistration>() {
              

            };
        }


        public  List<ITypeRegistration> GetStrategyRegistrations()
        {
            return new List<ITypeRegistration>() {
                new TypeRegistration<IComplexFilter, IComplexFilter>( LifeSpan.SingleInstance),
                new TypeRegistration<IComplexFilterAdapter, IComplexFilterAdapter>( LifeSpan.SingleInstance),

            };
        }
    }
}
