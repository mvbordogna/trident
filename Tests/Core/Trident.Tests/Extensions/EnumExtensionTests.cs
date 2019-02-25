using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Trident.Extensions;

namespace Trident.Tests.Extensions
{
    [TestClass]
    public class EnumExtensionsTests
    {
        private const string ExpectedDescription = "Hello";
        [TestMethod]
        public void EnumExtensions_GetDisplayValue_GetsDisplay_Return_Attribute_Value()
        {
            var actual = EnumExtensions.GetDisplayValue(TestEnum.Prop1);
            Assert.AreEqual(ExpectedDescription, actual);
        }

        
        [TestMethod]
        public void EnumExtensions_GetDisplayValue_Returns_Enum_String_Value_When_No_Attribute_Applied()
        {
            var actual = EnumExtensions.GetDisplayValue(TestEnum.Prop2);
            Assert.AreEqual(TestEnum.Prop2.ToString(), actual);
        }

        /// <exclude />
        private enum TestEnum
        {

            [Display(Name=ExpectedDescription)]
            Prop1 = 1,
            Prop2 = 2
        }

    }
}
