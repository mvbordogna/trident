using Microsoft.VisualStudio.TestTools.UnitTesting;
using Trident.Common;
using Trident.Contracts.Configuration;
using Trident.Testing.TestScopes;

namespace Trident.Tests.Common
{
    [TestClass]
    public class AppSettingsTests
    {
        [TestMethod]
        [Ignore]
        public void AppSettings_Retrieves_Value_Using_String_Key()
        {
            //setup
            var scope = new DefaultTestScope();

            //act
            var actual = scope.InstanceUnderTest[scope.TestKey];

            //assert
            Assert.AreEqual(scope.ExpectedTestKeyValue, actual);

        }

        [TestMethod]
        public void AppSettings_Returns_Null_When_Key_Not_Found()
        {
            //setup
            var scope = new DefaultTestScope();

            //act
            var actual = scope.InstanceUnderTest["BlaBla"];

            //assert
            Assert.IsNull(actual);
        }

        [TestMethod]
        [Ignore]
        public void AppSettings_Retrieves_Value_Using_Int_Index()
        {
            //setup
            var scope = new DefaultTestScope();

            //act
            var actual = scope.InstanceUnderTest[scope.TestIndex.ToString()];

            //assert
            Assert.AreEqual(scope.ExpectedTestKeyValue, actual);

        }

        [TestMethod]
        public void AppSettings_Returns_Null_When_Index_Not_Found()
        {
            //setup
            var scope = new DefaultTestScope();

            //act
            var actual = scope.InstanceUnderTest["100000000"];

            //assert
            Assert.IsNull(actual);
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
