using System;
using Trident.Data;
using Trident.Domain;

namespace Trident.TestTargetProject.Domain
{
    [UseSharedDataSource(DBSources.CosmosDB)]
    [Container("Organisations")]
    public class Organisation : EntityGuidBase
    {
        public string Name { get; set; }
        public DateTimeOffset Created { get; set; }

    }


}
