using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Trident.Workflow;
using Trident.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;
using Trident.Testing;
using Trident.Business;
using Trident.Logging;

namespace Trident.Tests.Workflow
{
    [TestClass]
    public class WorkflowManagerBaseTests
    {

        private string Task1Name = typeof(TestTask1).Name;
        private string Task2Name = typeof(TestTask2).Name;

        [TestMethod]
        [TestCategory(TestCategory.Unit)]
        public async Task WorkflowManagerBase_Automatically_Orders_Rules()
        {
            var testContext = new TestContext(new TestEntity(), new TestEntity());
            var instanceUnderTest = new TestWorkflowManager();
            await instanceUnderTest.Run(testContext);

            Assert.AreEqual(String.Concat("|",Task1Name,"|",Task2Name), testContext.Original.TestResult);
            Assert.AreEqual(String.Concat("|", Task1Name, "|", Task2Name), testContext.Target.TestResult);
        }

        [TestMethod]
        [TestCategory(TestCategory.Unit)]
        public async Task WorkflowManagerBase_DefaultConstructor_Automatically_Orders_Rules()
        {
            var testContext = new TestContext(new TestEntity(), new TestEntity());
            var instanceUnderTest = new TestWorkflowManager(true);
            await instanceUnderTest.Run(testContext);

            Assert.AreEqual(String.Concat("|", Task1Name, "|", Task2Name), testContext.Original.TestResult);
            Assert.AreEqual(String.Concat("|", Task1Name, "|", Task2Name), testContext.Target.TestResult);
        }

        [TestMethod]
        [TestCategory(TestCategory.Unit)]
        public async Task WorkflowManagerBase_Single_Entity_Context_Works()
        {
            var testContext = new TestContext(new TestEntity());
            var instanceUnderTest = new TestWorkflowManager(true);
            await instanceUnderTest.Run(testContext);

            Assert.IsNull(testContext.Original);
            Assert.AreEqual(String.Concat("|", Task1Name, "|", Task2Name), testContext.Target.TestResult);
        }

        private class TestWorkflowManager : WorkflowManagerBase<TestEntity>
        {
            public TestWorkflowManager() : base(new List<IWorkflowTask> {
                new TestTask2(),
                new TestTask1()
            }, new Moq.Mock<ILog>().Object)
            { }

            public TestWorkflowManager(bool defaultConstructor) : base(new IWorkflowTask[] {
                    new TestTask2(),
                    new TestTask1()
                }, new Moq.Mock<ILog>().Object) { }


            public TestWorkflowManager(int byPassAutoOrderingLogic) : base(new IWorkflowTask[] {
                   new TestPassingRule()
                }, new Moq.Mock<ILog>().Object)
            { }
        }

        private class BaseTestTask : WorkflowTaskBase<TestContext>
        {
            public override int RunOrder
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public override OperationStage Stage => OperationStage.All;

            protected virtual void AssignText(TestEntity entity)
            {
                if (entity != null)
                    entity.TestResult += "|"+this.GetType().Name;
            }

            public override async Task<bool> Run(TestContext context)
            {
                this.AssignText(context.Original);
                this.AssignText(context.Target);
                return await Task.FromResult(true);
            }       

            public override Task<bool> ShouldRun(TestContext context)
            {
                return Task.FromResult(true);
            }
        }

        private class TestTask1 : BaseTestTask
        {
            public override int RunOrder
            {
                get
                {
                    return 1;
                }
            }

        }


        private class TestTask2 : BaseTestTask
        {
            public override int RunOrder
            {
                get
                {
                    return 2;
                }
            }

        }

        private class TestNotRegisteredRule : BaseTestTask
        {
            public override int RunOrder
            {
                get
                {
                    return 2;
                }
            }

        }


        private class TestPassingRule : BaseTestTask
        {
            public override int RunOrder
            {
                get
                {
                    return 2;
                }
            }

        }


        public class TestContext : BusinessContext<TestEntity>
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="BusinessContext{T}"/> class.
            /// </summary>
            /// <param name="target">The target.</param>
            public TestContext(TestEntity target) : base(target) { }

            /// <summary>
            /// Initializes a new instance of the <see cref="BusinessContext{T}" /> class.
            /// </summary>
            /// <param name="target">The target.</param>
            /// <param name="original">The original.</param>
            public TestContext(TestEntity target, TestEntity original) : base(target, original) { }

        }

        public class TestEntity : Entity
        {
            public string TestResult { get; set; }

        }
    }
}
