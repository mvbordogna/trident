using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Trident.Validation;
using Trident.Domain;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Trident.Business;

namespace Trident.Tests.Validation
{
    [TestClass]
    public class ValidationManagerBaseTests
    {

        private const string Rule1PropertyText = "testRule1";
        private const string Rule2PropertyText = "testRule2";

        [TestMethod]
        public async Task ValidationManagerBase_Automatically_Orders_Rules()
        {
            var testContext = new TestContext(new TestEntity(), new TestEntity());
            var instanceUnderTest = new TestValidationManager();
            var results = await instanceUnderTest.CheckValid(testContext);
            var actualRule1Result = results[0].MemberNames.First();
            var actualRule2Result = results[1].MemberNames.First();

            Assert.AreEqual(Rule1PropertyText, actualRule1Result);
            Assert.AreEqual(Rule2PropertyText, actualRule2Result);
        }


        [TestMethod]
        public async Task ValidationManagerBase_Does_Not_Order_Rules()
        {
            var testContext = new TestContext(new TestEntity(), new TestEntity());
            var instanceUnderTest = new TestValidationManager(true);
            var results = await instanceUnderTest.CheckValid(testContext);
            var actualRule2Result = results[0].MemberNames.First();
            var actualRule1Result = results[1].MemberNames.First();

            Assert.AreEqual(Rule1PropertyText, actualRule1Result);
            Assert.AreEqual(Rule2PropertyText, actualRule2Result);
        }


        [TestMethod]
        public async Task ValidationManagerBase_CheckValid_Returns_Expected_Errors()
        {
            var testContext = new TestContext(new TestEntity(), new TestEntity());
            var instanceUnderTest = new TestValidationManager();
            var results = await instanceUnderTest.CheckValid(testContext);
            var actualRule1Result = results[0].MemberNames.First();
            var actualRule2Result = results[1].MemberNames.First();

            Assert.AreEqual(Rule1PropertyText, actualRule1Result);
            Assert.AreEqual(Rule2PropertyText, actualRule2Result);
        }

        [TestMethod]
        public async Task ValidationManagerBase_CheckValid_Returns_Expected_Error()
        {
            var testContext = new TestContext(new TestEntity(), new TestEntity());
            var instanceUnderTest = new TestValidationManager();
            var actual = await instanceUnderTest.CheckValid<TestRule1>(testContext);
            var actualRule1Result = actual.First().MemberNames.First();
            Assert.AreEqual(Rule1PropertyText, actualRule1Result);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public async Task ValidationManagerBase_CheckValid_Throws_ArgumentOutOfRange_Exception_If_Rule_Not_Found()
        {
            var testContext = new TestContext(new TestEntity(), new TestEntity());
            var instanceUnderTest = new TestValidationManager();
            await instanceUnderTest.CheckValid<TestNotRegisteredRule>(testContext);
        }

        [TestMethod]
        [ExpectedException(typeof(ValidationRollupException))]
        public async Task ValidationManagerBase_Validate_Throws_ValidationRollupException_If_Errors_Are_Added()
        {
            var testContext = new TestContext(new TestEntity(), new TestEntity());
            var instanceUnderTest = new TestValidationManager();
            await instanceUnderTest.Validate(testContext);
        }

        [TestMethod]
        [ExpectedException(typeof(ValidationRollupException))]
        public async Task ValidationManagerBase_Validate_Throws_ValidationRollupException_For_Specific_Rule_Requested_To_Run()
        {
            var testContext = new TestContext(new TestEntity(), new TestEntity());
            var instanceUnderTest = new TestValidationManager();
            await instanceUnderTest.Validate<TestRule1>(testContext);
        }

        [TestMethod]
        public async Task ValidationManagerBase_Validate_Does_Not_Throw_Exception_If_List_Is_Empty()
        {
            var testContext = new TestContext(new TestEntity(), new TestEntity());
            var instanceUnderTest = new TestValidationManager(0);
            await instanceUnderTest.Validate(testContext);
            //nothing to assert, if it throws the error then the test fails.
        }

        [TestMethod]
        public async Task ValidationManagerBase_Validate_Single_Rule_Does_Not_Throw_Exception_If_List_Is_Empty()
        {
            var testContext = new TestContext(new TestEntity(), new TestEntity());
            var instanceUnderTest = new TestValidationManager(0);
            await instanceUnderTest.Validate<TestPassingRule>(testContext);
            //nothing to assert, if it throws the error then the test fails.
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public async Task ValidationManagerBase_Validate_Throws_ArgumentOutOfRange_Exception_If_Rule_Not_Found()
        {
            var testContext = new TestContext(new TestEntity(), new TestEntity());
            var instanceUnderTest = new TestValidationManager();
            await instanceUnderTest.Validate<TestNotRegisteredRule>(testContext);
        }

        [TestMethod]

        public void ValidationContext_Get_Sets_Target_And_Original()
        {
            var expectedTarget = new TestEntity();
            var expectedOriginal = new TestEntity();
            var testContext = new TestContext(expectedTarget, expectedOriginal);

            Assert.AreEqual(expectedTarget, testContext.Target);
            Assert.AreEqual(expectedOriginal, testContext.Original);
        }



        private class TestValidationManager : ValidationManagerBase<TestEntity>
        {
            public TestValidationManager() : base(new List<IValidationRule> {
                new TestRule2(),
                new TestRule1()
            })
            { }

            public TestValidationManager(bool byPassAutoOrderingLogic) : base()
            {
                this.Rules.AddRange(new IValidationRule[] {
                    new TestRule2(),
                    new TestRule1()
                });
            }


            public TestValidationManager(int byPassAutoOrderingLogic) : base()
            {
                this.Rules.AddRange(new IValidationRule[] {
                   new TestPassingRule()
                });
            }
        }

        private class TestRule1 : ValidationRuleBase<TestContext>
        {
            public override int RunOrder
            {
                get
                {
                    return 1;
                }
            }

            public override async Task Run(TestContext context, List<ValidationResult> errors)
            {
                errors.Add(new ValidationResult<TestErrorCodes>(TestErrorCodes.TestCode, Rule1PropertyText));               
            }
        }


        private class TestRule2 : ValidationRuleBase<TestContext>
        {
            public override int RunOrder
            {
                get
                {
                    return 2;
                }
            }

            public override async Task Run(TestContext context, List<ValidationResult> errors)
            {
                errors.Add(new ValidationResult<TestErrorCodes>(TestErrorCodes.TestCode, Rule2PropertyText));
            }
        }

        private class TestNotRegisteredRule : ValidationRuleBase<TestContext>
        {
            public override int RunOrder
            {
                get
                {
                    return 2;
                }
            }

            public override async Task Run(TestContext context, List<ValidationResult> errors)
            {
                errors.Add(new ValidationResult<TestErrorCodes>(TestErrorCodes.TestCode, Rule2PropertyText));
            }
        }


        private class TestPassingRule : ValidationRuleBase<TestContext>
        {
            public override int RunOrder
            {
                get
                {
                    return 2;
                }
            }

            public override async Task Run(TestContext context, List<ValidationResult> errors)
            {

            }
        }


        public class TestContext : BusinessContext<TestEntity>
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="ValidationContext{T}"/> class.
            /// </summary>
            /// <param name="target">The target.</param>
            public TestContext(TestEntity target) : this(target, null) { }

            /// <summary>
            /// Initializes a new instance of the <see cref="ValidationContext{T}" /> class.
            /// </summary>
            /// <param name="target">The target.</param>
            /// <param name="original">The original.</param>
            public TestContext(TestEntity target, TestEntity original) : base(target, original) { }

        }

        public class TestEntity : EntityGuidBase
        {

        }      

    }
}
