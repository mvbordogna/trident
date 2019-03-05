using System;
using System.Collections.Generic;
using System.Text;

namespace Trident.Tests.Validation
{
    public enum TestErrorCodes
    {
      
        /// <summary>
        /// The test code -do not remove - unit test purposes
        /// </summary>
        [System.ComponentModel.Description("Test Code Message-needed for unit tests.")]
        TestCode = -1,
        /// <summary>
        /// The missing description test code -do not remove - unit test purposes
        /// </summary>
        MissingDescriptionTestCode = -2,

        [System.ComponentModel.Description("Unspecified Error")]
        Unspecified = 0
    }
}
