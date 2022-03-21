using KellermanSoftware.CompareNetObjects;
using KellermanSoftware.CompareNetObjects.TypeComparers;
using Newtonsoft.Json;
using Trident.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trident.Common
{
    /// <summary>
    /// Providers entity comparison configuration settings
    /// </summary>
    public class EntityCompareConfig
    {
        /// <summary>
        /// Gets or sets a value indicating whether to compare children member objects.
        /// </summary>
        /// <value><c>true</c> if [compare children]; otherwise, <c>false</c>.</value>
        public bool CompareChildren { get; set; } = false;

        /// <summary>
        /// A list of the string names of the members to be ignored
        /// </summary>
        /// <value>The members to ignore.</value>
        public List<string> MembersToIgnore { get; } = new List<string>();

        /// <summary>
        /// Gets the list of custom type comparers.
        /// </summary>
        /// <value>The custom type comparers.</value>
        /// <remarks>All custom type comarers to be used in the comparsions of two objects.</remarks>
        public List<BaseTypeComparer> CustomTypeComparers { get; } = new List<BaseTypeComparer>();
    }

    /// <summary>
    /// Performs a depp compare of Entities returning a list of differences between the two.
    /// Currently leveraging Compare-Net-Objects: See below link for more info.
    /// https://github.com/GregFinzer/Compare-Net-Objects
    /// Implements the <see cref="TridentOptionsBuilder.Common.IEntityComparer" />
    /// </summary>
    /// <seealso cref="TridentOptionsBuilder.Common.IEntityComparer" />
    public class EntityComparer : IEntityComparer
    {
        /// <summary>
        /// Compares the specified orignal and target entities using the specified configuration.
        /// </summary>
        /// <typeparam name="TEntity">The type of the t entity.</typeparam>
        /// <param name="original">The original.</param>
        /// <param name="target">The target.</param>
        /// <param name="config">The configuration.</param>
        /// <returns>IEnumerable&lt;EntityCompareResult&gt;.</returns>
        public IEnumerable<EntityCompareResult> Compare<TEntity>(TEntity original, TEntity target, EntityCompareConfig config) where TEntity : class
        {
            var results = new List<EntityCompareResult>();
            var comparer = new CompareLogic();
            comparer.Config.CompareChildren = config.CompareChildren;
            comparer.Config.MembersToIgnore.AddRange(config.MembersToIgnore);
            comparer.Config.MaxDifferences = int.MaxValue;
            comparer.Config.CustomComparers.AddRange(config.CustomTypeComparers);
            
            var compareResults = comparer.Compare(original, target);
            foreach (var difference in compareResults.Differences)
            {
                results.Add(new EntityCompareResult
                {
                    PropertyName = difference.PropertyName,
                    OriginalValue = difference.Object1Value,
                    NewValue = difference.Object2Value
                });
            }

            return results;
        }

        /// <summary>
        /// Creates a deep copy of the the specified entity
        /// </summary>
        /// <typeparam name="TEntity">The type of the t entity.</typeparam>
        /// <param name="entity">The entity.</param>
        /// <returns>TEntity.</returns>
        private TEntity SerializeDeserialize<TEntity>(TEntity entity)
       {
            return JsonConvert.DeserializeObject<TEntity>(JsonConvert.SerializeObject(entity));

       }
    }
}
