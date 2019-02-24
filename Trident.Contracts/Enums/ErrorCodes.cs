using System.ComponentModel;

namespace Trident.Contracts.Enums
{
    /// <summary>
    /// Enum ErrorCodes
    /// </summary>
    public enum ErrorCodes:int
    {
        /// <summary>
        /// The test code -do not remove - unit test purposes
        /// </summary>
        [Description("Test Code Message-needed for unit tests.")]
        TestCode = -1,
        /// <summary>
        /// The missing description test code -do not remove - unit test purposes
        /// </summary>
        MissingDescriptionTestCode = -2,
        /// <summary>
        /// The unspecified error
        /// </summary>
        UnspecifiedError = 0,
        /// <summary>
        /// The email already in use
        /// </summary>
        EmailAlreadyInUse =1,
        /// <summary>
        /// The external identifier already in use
        /// </summary>
        ExternalIdAlreadyInUse=2,     
      
        [Description("Password must be at least 10 characters.")]
        PasswordMinimumLength,

        [Description(" Invalid base url provided.")]
        InvalidUrl,

        [Description("Property Failed Validation.")]
        PropertyFailedVaildation,

        [Description("Insufficient Permisssions for requested action.")]
        InsufficientPermissions,		
    
    }
}
