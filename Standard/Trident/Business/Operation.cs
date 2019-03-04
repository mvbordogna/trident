namespace Trident.Business
{
    /// <summary>
    /// Enum Operation
    /// </summary>
    public enum Operation
    {
        /// <summary>
        /// The undefined
        /// </summary>
        Undefined = 0,
        /// <summary>
        /// The insert
        /// </summary>
        Insert = 1,
        /// <summary>
        /// The update
        /// </summary>
        Update = 2,
        /// <summary>
        /// The delete
        /// </summary>
        Delete = 3,
        /// <summary>
        /// defines that a custom operation
        /// Context Specific Rules will need expecit checking
        /// </summary>
        Custom = 4
    }
}
