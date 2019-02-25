using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Trident.Validation;
using Trident.Contracts.Enums;
using System.Linq;
using System.Reflection;

namespace Trident.Tests.Validation
{
    [TestClass]
    public class ValidationResultTests
    {
        [TestMethod]
        public void ValidationResult_Calculates_Object_Graph_Path_As_Dot_Notation_FirstCharacter_LowerCased()
        {
            var expected = "subTester.subTester.subTester.prop";
            var temp = new ValidationResult<TestEntity>(ErrorCodes.TestCode, x => x.SubTester.SubTester.SubTester.Prop);
            temp.ApplyExpression(temp.MemberExpressions.First());
            var list = temp.MemberNames.ToList();
            Assert.AreEqual(expected, list[0]);
        }

        [TestMethod]
        public void ValidationResult_Calculates_SingleProperty_Correctly()
        {
            var expected = "prop";
            var temp = new ValidationResult<TestEntity>(ErrorCodes.TestCode, x => x.Prop);
            temp.ApplyExpression(temp.MemberExpressions.First());
            var list = temp.MemberNames.ToList();
            Assert.AreEqual(expected, list[0]);
        }


        [TestMethod]
        public void ValidationResult_Calculates_Unary_Correctly()
        {
            var expected = "unaryLambdaConversionProperty";
            var temp = new ValidationResult<TestEntity>(ErrorCodes.TestCode, x => x.UnaryLambdaConversionProperty);
            temp.ApplyExpression(temp.MemberExpressions.First());
            var list = temp.MemberNames.ToList();
            Assert.AreEqual(expected, list[0]);
        }

        [TestMethod]
        public void ValidationResult_Calculates_Nested_Unary_Correctly()
        {
            var expected = "subTester.unaryLambdaConversionProperty";
            var temp = new ValidationResult<TestEntity>(ErrorCodes.TestCode, x => x.SubTester.UnaryLambdaConversionProperty);
            temp.ApplyExpression(temp.MemberExpressions.First());
            var list = temp.MemberNames.ToList();
            Assert.AreEqual(expected, list[0]);
        }


        [TestMethod]
        public void ValidationResult_Maps_Error_Code_Correctly()
        {
            var expected = ErrorCodes.TestCode;
            var actual = new ValidationResult<TestEntity>(ErrorCodes.TestCode, x => x.Prop);
            Assert.AreEqual(expected, actual.ErrorCode);
        }

        [TestMethod]
        public void ValidationResult_Maps_Message_String_Correctly()
        {
            var expected = "Test Code Message-needed for unit tests.";
            var actual = new ValidationResult<TestEntity>(ErrorCodes.TestCode, x => x.Prop);
            Assert.AreEqual(expected, actual.Message);
        }

        [TestMethod]
        public void ValidationResult_Maps_Message_String_With_No_Properties_Correctly()
        {
            var expected = "Test Code Message-needed for unit tests.";
            var actual = new ValidationResult(expected, ErrorCodes.TestCode);
            Assert.AreEqual(expected, actual.Message);
        }

        [TestMethod]
        public void ValidationResult_CamelCase_Convention_Handles_EmptyString()
        {
            var threwException = false;
            try
            {
                var actual = new ValidationResult(ErrorCodes.TestCode, string.Empty);
            }
            catch {
                threwException = true;
            }

            Assert.IsFalse(threwException);
        }

        [TestMethod]
        public void ValidationResult_ErrorCode_Only_Constructor_Has_Unspecified_Method()
        {
            var threwException = false;
            try
            {
                var actual = new ValidationResult(ErrorCodes.TestCode);
                Assert.IsTrue(actual.MemberNames.Count() > 0);
                Assert.IsTrue(actual.MemberNames.First() == "Unspecified");
            }
            catch
            {
                threwException = true;
            }

            Assert.IsFalse(threwException);
        }

        [TestMethod]
        public void ValidationResult_CamelCase_Convention_Handles_Null()
        {
            var threwException = false;
            try
            {
                var actual = new ValidationResult(ErrorCodes.TestCode, null as string);
            }
            catch
            {
                threwException = true;
            }

            Assert.IsFalse(threwException);
        }

        [TestMethod]
        public void ValidationResult_Sets_Default_Message_When_ErrorCode_Missing_Description()
        {
            var actual = new ValidationResult(ErrorCodes.MissingDescriptionTestCode, string.Empty);
            actual.Message.EndsWith(ErrorCodes.MissingDescriptionTestCode.ToString());
        }


        [TestMethod]
        public void ErrorCodes_All_Have_Description_Attribute()
        {
            var ignoredValue = ErrorCodes.MissingDescriptionTestCode.ToString();
            var enumValues = typeof(ErrorCodes).GetMembers(BindingFlags.Public | BindingFlags.Static);
            enumValues.ToList().ForEach(x =>
            {
                if (x.Name != ignoredValue)
                {
                    var descriptionLength = x.GetCustomAttribute<System.ComponentModel.DescriptionAttribute>()?.Description?.Length ?? 0;
                    Assert.AreNotEqual(0, descriptionLength,
                        $"ErrorCode.{x.Name} is missing the required description attribute with a value for the description.");
                }
            });

        }





        public class TestEntity
        {
            public TestEntity SubTester { get; set; }

            public string Prop { get; set; }

            public decimal UnaryLambdaConversionProperty { get; set; }
        }
    }
}
