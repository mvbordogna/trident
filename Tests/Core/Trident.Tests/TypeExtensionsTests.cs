using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Trident.Tests
{
    [TestClass]
    public class TypeExtensionsTests
    {
        [TestMethod]
        public void TypeExtensions_GetDirectlyImplementedInterfaces_GetsExpectedInterface()
        {
            //setup
            var expected = typeof(ITestInterfaceDerived);
            //act
            var actual = TypeExtensions.GetDirectlyImplementedInterfaces(typeof(TestClassDerived));

            //assert
            Assert.AreEqual(1, actual.Count());
            Assert.IsTrue(actual.Contains(expected));
        }

        [TestMethod]
        public void TypeExtensions_GetDirectlyImplementedInterfaces_Does_Not_Return_Inherited_Interface_From_SuperClass()
        {
            //setup
            var expected = typeof(ITestInterfaceSuperClass);

            //act
            var actual = TypeExtensions.GetDirectlyImplementedInterfaces(typeof(TestClassDerived));

            //asert
            Assert.IsFalse(actual.Contains(expected));
        }

        [TestMethod]
        public void TypeExtensions_GetDirectlyImplementedInterfaces_Does_Not_Return_Inherited_Interface_From_Implemented_Interface()
        {
            //setup
            var expected = typeof(ITestInterfaceParent);

            //act
            var actual = TypeExtensions.GetDirectlyImplementedInterfaces(typeof(TestClassDerived));

            //assert
            Assert.IsFalse(actual.Contains(expected));
        }

        [TestMethod]
        public void TypeExtensions_GetParserFunction_Returns_Supported_Privative_Parse_Functions()
        {
            //setup
            var testScope = new DefaultTestScope();
            var supportedTypesWithTestValues = testScope.SupportedTypesWithTestValues;
            var keys = supportedTypesWithTestValues.Keys.ToList();

            //act
            keys.ForEach(k =>
            {

                var func = TypeExtensions.GetParserFunction(k);
                var actual = func(supportedTypesWithTestValues[k]);

                //assert
                Assert.AreEqual(supportedTypesWithTestValues[k].ToString(), actual.ToString());
            });
        }

        [TestMethod]
        public void TypeExtensions_GetParserFunction_Supports_Dynamic_Enum_Requests()
        {
            //setup
            var expected = TestEnum.Other;
            var testVal = expected.ToString();

            //act
            var func = TypeExtensions.GetParserFunction(typeof(TestEnum));
            var actual = func(testVal);

            //assert
            Assert.AreEqual(expected, (TestEnum)actual);

        }


        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void TypeExtensions_GetParserFunction_Throws_NotSupported_Exception_For_Unsupported_Types()
        {
            //setup
            var expected = TestEnum.Other;
            var testVal = expected.ToString();

            //act
            var func = TypeExtensions.GetParserFunction(typeof(object));           
        }

        [TestMethod]
        public void TypeExtensions_IsPrivative_Returns_Expected_Value_For_Extended_Privative_Types()
        {
            var testScope = new DefaultTestScope();
            var primitives = testScope.TreatAsPrimitive.ToList();
                      
            primitives.ForEach(p =>
            {
                //act
                var actual = TypeExtensions.IsPrimitive(p);

                //assert
                Assert.IsTrue(actual);
            });
        }
        
        [TestMethod]
        public void TypeExtensions_ParseEnum_Returns_Expected_Enum_Value()
        {
            //setup
            TestEnum expected = TestEnum.SomeVal;
            string testVal = expected.ToString();

            //act
            var actual = TypeExtensions.ParseEnum<TestEnum>(testVal);

            //assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TypeExtensions_ParseEnum_Returns_Default_Enum_Value_When_Input_is_Null()
        {
            //setup
            TestEnum expected = TestEnum.NotSet;
            string testVal = expected.ToString();

            //act
            var actual = TypeExtensions.ParseEnum<TestEnum>(null);

            //assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TypeExtensions_ParseToType_Parses_ListOfPrimativesInCsvFormat()
        {
            //setup
            string testValue = "1,2,3";
            var expected = new List<int>() {1, 2, 3 };
            string testVal = expected.ToString();

            //act
            var actual = (ICollection)TypeExtensions.ParseToTypedObject(testValue, typeof(IEnumerable<int>));

            //assert          

            CollectionAssert.AreEquivalent(expected, actual);
        }


        private class TestType
        {
            public string Name { get; set; }
            public string Phone { get; set; }
        }

        [TestMethod]
        public void TypeExtensions_ParseToType_Parses_ListOfComplexTypesInJsonListFormat()
        {
            //setup
            string testValue = " [{ \"name\": \"Bob\", \"phone\": \"444-444-4444\"},{ \"name\": \"Tony Two-Toes\", \"phone\": \"666-444-4444\"},{ \"name\": \"steve\", \"phone\": \"555-444-4444\"}] ";
            var expected = new List<TestType>() {
                new TestType(){ Name = "Bob", Phone = "444-444-4444" },
                new TestType(){ Name = "Tony Two-Toes", Phone = "666-444-4444" },
                new TestType(){ Name = "steve", Phone = "555-444-4444" },
            };
            string testVal = expected.ToString();

            //act
            var actual = (ICollection)TypeExtensions.ParseToTypedObject(testValue, typeof(IEnumerable<TestType>));

            //assert     
            Assert.IsNotNull(actual);
            Assert.IsTrue(typeof(IEnumerable<TestType>).IsAssignableFrom(actual.GetType()));
            Assert.AreEqual(expected.Count, actual.Count);
        }


        private class DefaultTestScope
        {


            public Type[] TreatAsPrimitive { get; } = new[]
            {
                typeof (Enum),
                typeof (String),
                typeof (Char),
                typeof (Guid),
                typeof (Boolean),
                typeof (Byte),
                typeof (Int16),
                typeof (Int32),
                typeof (Int64),
                typeof (Single),
                typeof (Double),
                typeof (Decimal),
                typeof (SByte),
                typeof (UInt16),
                typeof (UInt32),
                typeof (UInt64),
                typeof (DateTime),
                typeof (DateTimeOffset),
                typeof (TimeSpan),
                typeof(TestEnum),

                typeof (Char?),
                typeof (Guid?),
                typeof (Boolean?),
                typeof (Byte?),
                typeof (Int16?),
                typeof (Int32?),
                typeof (Int64?),
                typeof (Single?),
                typeof (Double?),
                typeof (Decimal?),
                typeof (SByte?),
                typeof (UInt16?),
                typeof (UInt32?),
                typeof (UInt64?),

                typeof (DateTime?),
                typeof (DateTimeOffset?),
                typeof (TimeSpan?),
                typeof(TestEnum?)
            };


            public Dictionary<Type, string> SupportedTypesWithTestValues { get; } = new Dictionary<Type, string>()
            {
                {typeof(string),"TestString"},
                { typeof(Guid), Guid.NewGuid().ToString() },
                {typeof(DateTime), DateTime.Now.ToString() },
                {typeof(char),'a'.ToString() },
                {typeof(SByte), SByte.MaxValue.ToString() },
                {typeof(byte), byte.MaxValue.ToString() },
                {typeof(long), long.MaxValue.ToString() },
                {typeof(int), int.MaxValue.ToString() },
                {typeof(short), short.MaxValue.ToString() },
                {typeof(ushort), ushort.MaxValue.ToString() },
                {typeof(ulong), ulong.MaxValue.ToString() },
                {typeof(uint), uint.MaxValue.ToString() },
                {typeof(float), float.MaxValue.ToString() },
                {typeof(double),  (double.MaxValue *100).ToString() },
                {typeof(decimal), decimal.MaxValue.ToString() },
                { typeof(DateTimeOffset), DateTimeOffset.MaxValue.ToString() },
                { typeof(TimeSpan), TimeSpan.MaxValue.ToString() },
                                       
                //nullable
                { typeof(Guid?), Guid.NewGuid().ToString() },
                { typeof(DateTime?),  DateTime.Now.AddDays(1).ToString() },
                {typeof(char?), 'b'.ToString()},
                {typeof(SByte?), SByte.MaxValue.ToString() },
                {typeof(byte?), byte.MaxValue.ToString() },
                {typeof(long?), long.MaxValue.ToString() },
                {typeof(int?), int.MaxValue.ToString() },
                {typeof(short?), short.MaxValue.ToString() },
                {typeof(ushort?), ushort.MaxValue.ToString() },
                {typeof(ulong?), ulong.MaxValue.ToString() },
                {typeof(uint?), uint.MaxValue.ToString() },
                {typeof(float?),float.MaxValue.ToString() },
                {typeof(double?), (double.MaxValue * 100).ToString() },
                { typeof(decimal?), decimal.MaxValue.ToString() },
                { typeof(DateTimeOffset?), DateTimeOffset.MaxValue.ToString() },
                { typeof(TimeSpan?), TimeSpan.MaxValue.ToString() },
            };


        }

        #region Test Structures

        private enum TestEnum
        {
            NotSet = 0,
            SomeVal = 1,
            Other = 2
        }

        private interface ITestInterfaceSuperClass { }
        private interface ITestInterfaceDerived : ITestInterfaceParent { }
        private interface ITestInterfaceParent { }
        private class TestSuperClass : ITestInterfaceSuperClass { }
        private class TestClassDerived : TestSuperClass, ITestInterfaceDerived { }

        #endregion
    }
}
