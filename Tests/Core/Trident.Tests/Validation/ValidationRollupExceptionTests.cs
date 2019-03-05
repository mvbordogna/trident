using Microsoft.VisualStudio.TestTools.UnitTesting;
using Trident.Validation;
using System.Linq;
using Trident.Contracts.Enums;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System;
using Trident.Domain;

namespace Trident.Tests.Validation
{
    [TestClass]
    public class ValidationRollupExceptionTests
    {
        [TestMethod]
        public void ValidationRollupException_Adds_Errors()
        {
            var expected = new ValidationResult<TestErrorCodes>(TestErrorCodes.TestCode);
            var ex = new ValidationRollupException();
            ex.AddResult(expected);

            Assert.AreEqual(expected, ex.ValidationResults.First());
        }

        [TestMethod]
        public void ValidationRollupException_Constructor_Adds_Error()
        {
            var expected = new ValidationResult<TestErrorCodes>(TestErrorCodes.TestCode);
            var ex = new ValidationRollupException(expected);
            Assert.AreEqual(expected, ex.ValidationResults.First());
        }

        [TestMethod]
        public void ValidationRollupException_Constructor_Adds_Error_from_Primitive_Arguments()
        {
            var expectedMember = "member";
            var expectedCode = TestErrorCodes.TestCode;
            var ex = new ValidationRollupException<TestErrorCodes, TestEntity>(expectedMember, expectedCode);            

            Assert.AreEqual(expectedMember, ex.ValidationResults.First().MemberNames.First());
            Assert.AreEqual("Test Code Message-needed for unit tests.", ex.ValidationResults.First().Message);
        }

        [TestMethod]
        public void ValidationRollupException_Constructor_Adds_Message_with_Errors()
        {
            var expectedMessage = "member";
            var expectedResult = new ValidationResult<TestErrorCodes>(TestErrorCodes.TestCode);
          
            var ex = new ValidationRollupException(expectedMessage, new ValidationResult[] { expectedResult });

            Assert.AreEqual(expectedMessage, ex.Message);
            Assert.AreEqual(expectedResult, ex.ValidationResults.First());
        }

        [TestMethod]
        public void ValidationRollupException_Constructor_Sets_Default_Message_When_Argument_Is_Empty_String()
        {
            var ex = new ValidationRollupException(string.Empty);
            Assert.AreEqual(ValidationRollupException.DefaultErrorMessage, ex.Message);
        }

        [TestMethod]
        public void ValidationRollupException_Serializes()
        {
            var ex = new ValidationRollupException(string.Empty);

            MemoryStream memoryStream = new MemoryStream();
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            binaryFormatter.Serialize(memoryStream, ex);
            memoryStream.Flush();
            memoryStream.Position = 0;
            var result = binaryFormatter.Deserialize(memoryStream);
        }


        [TestMethod]
        public void ValidationRollupException_Constructor_Sets_Internals_Correctly()
        {
            var expectedMessage = "the message";
            var expectedMember = "member";
            var expectedResultMessage = "Test Code Message-needed for unit tests.";
            var expectedCode = TestErrorCodes.TestCode; 
            var ex = new ValidationRollupException<TestErrorCodes, TestEntity>(expectedMessage, expectedMember, expectedCode);
            var actual = ex.ValidationResults.First();

            Assert.AreEqual(expectedMessage, ex.Message);
            Assert.AreEqual(expectedMember, actual.MemberNames.First());
            Assert.AreEqual(expectedResultMessage, actual.Message);
            Assert.AreEqual(expectedCode, actual.ErrorCode);
        }

      

        public class TestEntity : Entity
        {

        }


    }
}
