using AutoMapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Trident.Mapper;
using Trident.Validation;
using System.Collections.Generic;
using System.Linq;
using Trident.Domain;

namespace Trident.Tests.Mapper
{
    [TestClass]
    public class AutoMapperRegistryTests
    {
        [TestMethod]
        public void AutoMapperRegistry_MapPropertyExpressions_Maps_Simple_Lambda_Expression_Correctly()
        {
            var expected = "simpleLambdaMapProperty";

            var instanceUnderTest = new TestMapperRegistry();
            var testData = new List<ValidationResult<TestErrorCodes, TestEntity>>()
            {
                new ValidationResult<TestErrorCodes, TestEntity>(TestErrorCodes.TestCode, x=> x.SimpleLambdaMapProperty)
            };
            instanceUnderTest.MapPropertyExpressions<TestErrorCodes, TestEntity, TestDto>(testData);
            Assert.AreEqual(expected, testData[0].MemberNames.First());
        }


        [TestMethod]
        public void AutoMapperRegistry_MapPropertyExpressions_Maps_Lambda_Convert_Expression_Correctly()
        {
            var instanceUnderTest = new TestMapperRegistry();
            var expected = "unMappedProperty";

            var testData = new List<ValidationResult<TestErrorCodes, TestEntity>>()
            {
                new ValidationResult<TestErrorCodes, TestEntity>(TestErrorCodes.TestCode, x=> x.UnMappedProperty)
            };
            instanceUnderTest.MapPropertyExpressions<TestErrorCodes, TestEntity, TestDto>(testData);
            Assert.AreEqual(expected, testData[0].MemberNames.First());
        }



        [TestMethod]
        public void AutoMapperRegistry_MapPropertyExpressions_Maps_Lambda_For_Nested_Reduce_Mapping_Expression_Correctly()
        {
            var expected = "nestedMapProperty";

            var instanceUnderTest = new TestMapperRegistry();

            var testData = new List<ValidationResult<TestErrorCodes, TestEntity>>()
            {
                new ValidationResult<TestErrorCodes, TestEntity>(TestErrorCodes.TestCode,  x=> x.NestedProp1.NestedMapProperty)
            };
            instanceUnderTest.MapPropertyExpressions<TestErrorCodes, TestEntity, TestDto>(testData);
            Assert.AreEqual(expected, testData[0].MemberNames.First());
        }

        [TestMethod]
        public void AutoMapperRegistry_MapPropertyExpressions_Maps_Entity_Expression_When_Mapping_Definition_is_Function()
        {
            var expected = "propWithFunctionExpression";

            var instanceUnderTest = new TestMapperRegistry();
            var testData = new List<ValidationResult<TestErrorCodes, TestEntity>>()
            {
                new ValidationResult<TestErrorCodes, TestEntity>(TestErrorCodes.TestCode, x=> x.PropWithFunctionExpression)
            };
            instanceUnderTest.MapPropertyExpressions<TestErrorCodes, TestEntity, TestDto>(testData);
            Assert.AreEqual(expected, testData[0].MemberNames.First());
        }

        [TestMethod]
        public void AutoMapperRegistry_MapPropertyExpressions_Maps_SubObject_Property_Expression()
        {
            var expected = "nestedProp1.simpleLambdaMapProperty";
            var instanceUnderTest = new TestMapperRegistry();
            var testData = new List<ValidationResult<TestErrorCodes, TestEntity>>()
            {
                new ValidationResult<TestErrorCodes, TestEntity>(TestErrorCodes.TestCode, x=> x.NestedProp1.SimpleLambdaMapProperty)
            };
            instanceUnderTest.MapPropertyExpressions<TestErrorCodes, TestEntity, TestDto>(testData);
            Assert.AreEqual(expected, testData[0].MemberNames.First());
        }


        [TestMethod]
        public void AutoMapperRegistry_MapPropertyExpressions_Uses_Explicitly_Set_String_Converted_To_CamelCase()
        {
            var expected = "myExplicitPropertyName";
            var instanceUnderTest = new TestMapperRegistry();
            var testData = new List<ValidationResult<TestErrorCodes, TestEntity>>()
            {
                new ValidationResult<TestErrorCodes, TestEntity>(TestErrorCodes.TestCode, "MyExplicitPropertyName")
            };
            instanceUnderTest.MapPropertyExpressions<TestErrorCodes,TestEntity, TestDto>(testData);
            Assert.AreEqual(expected, testData[0].MemberNames.First());
        }


        private class TestMapperRegistry : AutoMapperRegistry
        {
            public TestMapperRegistry() : base(new MapperConfiguration(cfg =>
            {

                CreateTestMaps(cfg);
            }))
            {

            }

            private static void CreateTestMaps(IMapperConfigurationExpression cfg)
            {
                cfg.CreateMap<TestEntity, TestDto>()
                   .ForMember(x => x.SimpleLambdaMapProperty, opt => opt.MapFrom(x => x.SimpleLambdaMapProperty))
                   .ForMember(x => x.NestedMapProperty, opt => opt.MapFrom(x => x.NestedProp1.NestedMapProperty))
                   .ForMember(x => x.PropWithFunctionExpression, opt => opt.MapFrom(x => string.IsNullOrEmpty(x.PropWithFunctionExpression) ? null : x.PropWithFunctionExpression));

                cfg.CreateMap<TestNestedEntity, TestNestedDto>()
                    .ForMember(x => x.SimpleLambdaMapProperty, opt => opt.MapFrom(x => x.SimpleLambdaMapProperty));
            }

        }

        private class TestDto
        {

            public string SimpleLambdaMapProperty { get; set; }

            public decimal UnMappedProperty { get; set; }

            public decimal NestedMapProperty { get; set; }

            public string PropWithFunctionExpression { get; set; }

            public TestNestedDto NestedProp1 { get; set; }
        }


        public class TestNestedDto
        {

            public string SimpleLambdaMapProperty { get; set; }

            public decimal UnMappedProperty { get; set; }

            public decimal NestedMapProperty { get; set; }

            public string PropWithFunctionExpression { get; set; }

        }


        public class TestEntity : Entity
        {

            public string SimpleLambdaMapProperty { get; set; }

            public decimal UnMappedProperty { get; set; }

            public decimal NestedMapProperty { get; set; }

            public string PropWithFunctionExpression { get; set; }

            public TestNestedEntity NestedProp1 { get; set; }
        }

        public class TestNestedEntity
        {
            public string SimpleLambdaMapProperty { get; set; }

            public decimal UnMappedProperty { get; set; }

            public decimal NestedMapProperty { get; set; }

            public string PropWithFunctionExpression { get; set; }

        }


        public enum TestErrorCodes
        {
            Unspecified = 0,
            TestCode = 1,
            Error2 = 2
        }

    }
}
