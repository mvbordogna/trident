using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Trident.Extensions;

namespace Trident.Tests.Extensions
{
    [TestClass]
    public class ObjectExtensionsTests
    {
        private const string ExpectedDescription = "Hello";

        [TestMethod]
        public void ObjectExtensions_GetDisplayValue_GetsDisplay_Return_Null_When_Passed_Null()
        {
            //setup
            var scope = new DefaultTestScope();
            TestSerializationTarget orig = null;

            //act
            var clone = ObjectExtensions.Clone(orig);

            //assert
            Assert.IsNull(clone);       
        }

        [TestMethod]
        public void ObjectExtensions_GetDisplayValue_GetsDisplay_Return_Attribute_Value()
        {
            //setup
            var scope = new DefaultTestScope();

            //act
            var clone = ObjectExtensions.Clone(scope.TestSerializationTarget);

            //assert
            Assert.IsNotNull(clone);
            Assert.IsInstanceOfType(clone, typeof(TestSerializationTarget));
            Assert.AreEqual(scope.TestSerializationTarget.Data, clone.Data);
        }


        [TestMethod]
        public void ObjectExtensions_GetDisplayValue_Returns_Enum_String_Value_When_No_Attribute_Applied()
        {
            //setup
            var scope = new DefaultTestScope();

            //act
            var clone = ObjectExtensions.Clone(scope.TestJsonSerializationTarget);

            //assert
            Assert.IsNotNull(clone);
            Assert.IsInstanceOfType(clone, typeof(TestJsonSerializationTarget));
            Assert.AreEqual(scope.TestJsonSerializationTarget.Data, clone.Data);
        }



        private class DefaultTestScope
        {

            public TestSerializationTarget TestSerializationTarget { get; }

            public TestJsonSerializationTarget TestJsonSerializationTarget { get; }

            public DefaultTestScope()
            {
                TestSerializationTarget = new TestSerializationTarget()
                {
                    Data = "Serialization Test"
                };

                TestJsonSerializationTarget = new TestJsonSerializationTarget()
                {
                    Data = "Json Serialization Test"
                };
            }
        }



        [Serializable]
        private class TestSerializationTarget
        {

            public string Data { get; set; }
        }

        private class TestJsonSerializationTarget
        {
            public string Data { get; set; }

        }

    }
}
