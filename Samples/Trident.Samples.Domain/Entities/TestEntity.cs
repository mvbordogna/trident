using Trident.Data;
using Trident.Domain;

namespace Trident.Samples.Domain.Entities
{
    [UseSharedDataSource(DBSources.CosmosDB)]
    public class TestEntity : EntityGuidBase
    {
        public string Name { get; set; }
    }
}
