using Trident.Data;
using Trident.Domain;

namespace Trident.TestTargetProject.Domain
{
    [UseSharedDataSource(DBSources.CosmosDB)]
    public class TestEntity : EntityGuidBase
    {
        public string Name { get; set; }
    }
}                                                                
