using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Cosmos.Infrastructure.Internal;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Trident.Common;
using Trident.Data;
using Trident.EFCore.Contracts;
using Trident.IoC;
using Trident.Logging;

namespace Trident.EFCore
{
    public class EFCoreCosmosDataContext : EFCoreDataContext
    {
        public EFCoreCosmosDataContext(
            IEFCoreModelBuilderFactory modelBuilderFactory,
            IEntityMapFactory mapFactory,
            string dataSource,
            DbContextOptions options,
            ILog log,
            ILoggerFactory loggerFactory,
            IAppSettings appSettings,
            IIoCServiceLocator ioCServiceLocator
            ) : base(modelBuilderFactory, mapFactory, dataSource, options, log, loggerFactory, appSettings, ioCServiceLocator) { }

        public override async Task<IEnumerable<TEntity>> ExecuteQueryAsync<TEntity>(string command, IDictionary<string, object> parameters = null) where TEntity : class
        {
            var client = GetDbClient<TEntity>();
            var dbInfo = GetDataSourceInfo<TEntity>();
            var results = (await client.ExecuteQueryAsync(command, parameters)).ToList();
            results.ForEach(x => x["id"] = x["id"].ToString().Replace($"{dbInfo.DiscriminatorValue}|", ""));
            return JsonConvert.DeserializeObject<IEnumerable<TEntity>>(JsonConvert.SerializeObject(results));
        }

        public override async Task<int> SaveChangesAsync<TEntity>(CancellationToken cancellationToken = default)
        {
            IEnumerable<EntityEntry<TEntity>> itemsToUpdate = null;

            _log.Debug<EFCoreCosmosDataContext>(messageTemplate: $"Calling {nameof(SaveChangesAsync)}<TEntity> on context:" +
                $" {this.ContextId} with provider: {this.Database.ProviderName}");

            if (ChangeTracker.HasChanges())
            {
                _log.Debug<EFCoreCosmosDataContext>(messageTemplate: $"HasChanges");

                var properties = typeof(TEntity).GetProperties(BindingFlags.Instance | BindingFlags.Public)
                     .Select(x => new { PropertyInfo = x, Attr = x.GetCustomAttribute<ManuallyTackedAttribute>() })
                     .Where(x => x.Attr != null);

                if (properties.Any())
                {
                    itemsToUpdate = this.ChangeTracker.Entries<TEntity>()
                        .Where(x => x.State == EntityState.Modified || x.State == EntityState.Added)
                        .ToList();
                }

                var result = await base.SaveChangesAsync(cancellationToken);
                _log.Debug<EFCoreCosmosDataContext>(messageTemplate: $"Primary SaveChangesResult: {result}");
                if (itemsToUpdate != null)
                {
                    foreach (var entry in itemsToUpdate)
                    {
                        var jsonProperty = entry.Property<JObject>("__jObject");
                        if (jsonProperty.CurrentValue == null) continue;


                        foreach (var item in properties)
                        {
                            var memberKey = !string.IsNullOrWhiteSpace(item.Attr.Name)
                               ? item.Attr.Name
                               : item.PropertyInfo.Name;

                            var value = item.PropertyInfo.GetValue(entry.Entity);
                            var expConverter = new ExpandoObjectConverter();
                            var jsonStr = JsonConvert.SerializeObject(value, expConverter);
                            jsonProperty.CurrentValue[memberKey] = JToken.Parse(jsonStr);
                        }
                        entry.State = EntityState.Modified;
                    }

                    result = await base.SaveChangesAsync(cancellationToken);
                    _log.Debug<EFCoreCosmosDataContext>(messageTemplate: $"Dynamic Object update SaveChangesResult: {result}");
                }

                return result;
            }
            return 0;
        }

        public override void MapDynamicObjects<TEntity>(TEntity entity)
        {
            if (entity == null) return;

            var entry = this.Entry(entity);
            var jsonProperty = entry.Property<JObject>("__jObject");

            var properties = typeof(TEntity).GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Select(x => new { PropertyInfo = x, Attr = x.GetCustomAttribute<ManuallyTackedAttribute>() })
                .Where(x => x.Attr != null);


            foreach (var item in properties)
            {
                var memberKey = !string.IsNullOrWhiteSpace(item.Attr.Name)
                   ? item.Attr.Name
                   : item.PropertyInfo.Name;

                if (jsonProperty.CurrentValue.ContainsKey(memberKey))
                {
                    var childMemberVal = jsonProperty.CurrentValue[memberKey];
                    var expConverter = new ExpandoObjectConverter();
                    dynamic obj = JsonConvert.DeserializeObject<ExpandoObject>(childMemberVal.ToString(), expConverter);
                    item.PropertyInfo.SetValue(entity, obj);
                }
            }
        }

        public override IDbClient<T> GetDbClient<T>()
        {
            var dbInfo = GetDataSourceInfo<T>();
            var cosmosClient = this.Database.GetCosmosClient();
            var database = cosmosClient.GetDatabase(dbInfo.DatabaseName);
            var container = database.GetContainer(dbInfo.Container);

            return new CosmosDbClient<T>(container, dbInfo);
        }

        private DataSourceInfo GetDataSourceInfo<T>()
        {
            var type = typeof(T);
            var containerAttr = type.GetCustomAttribute<ContainerAttribute>();
            var discriminatorAttr = type.GetCustomAttribute<DiscriminatorAttribute>();
            return new DataSourceInfo()
            {
                DatabaseName = GetDatabaseName(),
                Container = containerAttr?.Name,
                PartitionKey = containerAttr?.PartitionKey,
                DiscriminatorProperty = discriminatorAttr.Property,
                DiscriminatorValue = discriminatorAttr.Value,
                TargetEntityType = typeof(T)
            };
        }


        private string GetDatabaseName()
        {
            var optionExtension = (CosmosOptionsExtension)this.Options.Extensions.FirstOrDefault(x => x.GetType() == typeof(CosmosOptionsExtension));
            return optionExtension.DatabaseName;
        }




    }
}
