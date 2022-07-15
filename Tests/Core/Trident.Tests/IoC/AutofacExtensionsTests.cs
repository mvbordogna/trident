using Microsoft.VisualStudio.TestTools.UnitTesting;
using Trident.Common;
using Trident.Contracts.Configuration;
using Trident.Testing.TestScopes;

namespace Trident.Tests.Common
{
    [TestClass]
    public class AutofacExtensionsTests
    {
        [TestMethod]
        [Ignore]
        public void Autofac_Retrieves_Value_Using_String_Key()
        {



           
               
            //setup
            var scope = new DefaultTestScope();

            //act
            var actual = scope.InstanceUnderTest[scope.TestKey];

            //assert
            Assert.AreEqual(scope.ExpectedTestKeyValue, actual);

        }


        private class DefaultTestScope : TestScope<IAppSettings>
        {
            public int TestIndex { get; } = 0;
            public string TestKey { get; } = "TestSetting";
            public string ExpectedTestKeyValue = "Expected";

            public DefaultTestScope()
            {
                InstanceUnderTest = new XmlAppSettings();
            }
        }

    }
}
