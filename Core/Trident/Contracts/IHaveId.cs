namespace Trident.Contracts
{
    /// <summary>
    /// A marker interface that allows for multiple inheritance between EntityBase and TridentDualIdentifierEntityBase
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IHaveId<T>
    {
        /// <summary>
        /// Gets the primary identifier for this instance.
        /// </summary>
        /// <returns>T.</returns>
        T GetId();
    }
}