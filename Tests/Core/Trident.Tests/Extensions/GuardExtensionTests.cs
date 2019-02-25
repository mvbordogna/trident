using Microsoft.VisualStudio.TestTools.UnitTesting;
using Trident.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trident.Tests.Extensions
{    [TestClass]
    public class GuardExtensionsTests
    {
        [TestMethod]
        public void GuardExtensions_GuardIsNotNull_Throws_ArgumentNullException_When_Null_And_Includes_Parameter_Name()
        {
            object test = null;

            try
            {              
                GuardExtensions.GuardIsNotNull(test, nameof(test));
            }
            catch (ArgumentNullException ex)
            {
                Assert.AreEqual(ex.ParamName, nameof(test));
            }
        }

        [TestMethod]
        public void GuardExtensions_GuardIsNotNull_Does_Not_Throw_ArgumentNullException_When_Has_Value()
        {
            object test = new object();

            try
            {
                GuardExtensions.GuardIsNotNull(test, nameof(test));                
            }
            catch (ArgumentNullException)
            {
                Assert.Fail("Unexpected ArgumentNullException was thrown in Guard.GuardIsNotNull(this object, string) method.");
            }
        }


        [TestMethod]
        public void GuardExtensions_GuardAreEqual_Throws_ArgumentNullException_When_Null_And_Includes_Parameter_Name()
        {
            int ObjA = 10;
            int objB = ObjA;

            try
            {
                GuardExtensions.GuardAreEqual(objB, objB, nameof(objB));
            }
            catch (ArgumentNullException ex)
            {
                Assert.AreEqual(ex.ParamName, nameof(objB));
            }
        }

        [TestMethod]
        public void GuardExtensions_GuardAreEqual_Does_Not_Throw_ArgumentNullException_When_Has_Value()
        {
            object test = new object();

            try
            {
                GuardExtensions.GuardIsNotNull(test, nameof(test));
            }
            catch (ArgumentNullException)
            {
                Assert.Fail("Unexpected ArgumentNullException was thrown in Guard.GuardIsNotNull(this object, string) method.");
            }
        }       
    }
}
