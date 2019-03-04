using System;

namespace Trident.Workflow
{
    /// <summary>
    /// Enum OperationStage
    /// </summary>
    [Flags]
    public enum OperationStage
    {
        /// <summary>
        /// All
        /// </summary>
        All = 0,
        /// <summary>
        /// The post validation
        /// </summary>
        PostValidation = 1,
        /// <summary>
        /// The before delete
        /// </summary>
        BeforeDelete = 2,
        /// <summary>
        /// The after delete
        /// </summary>
        AfterDelete = 4,
        /// <summary>
        /// The before insert
        /// </summary>
        BeforeInsert = 8,
        /// <summary>
        /// The AfterInsert event occurs after a new record is added.
        /// </summary>
        AfterInsert = 16,
        /// <summary>
        /// The before update
        /// </summary>
        BeforeUpdate = 32,
        /// <summary>
        /// The after update
        /// </summary>
        AfterUpdate = 64,
        //represents a custon stage, 
        //enable if using workflow in a custom manager method
        Custom = 128
    }
}
