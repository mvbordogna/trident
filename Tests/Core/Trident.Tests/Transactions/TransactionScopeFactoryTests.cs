using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Trident.Common;
using Trident.Transactions;
using System.Reflection;
using System.Transactions;
using Trident.Contracts.Configuration;

namespace Trident.Tests.Transactions
{
    [TestClass]
    public class TransactionScopeFactoryTests
    {
        [TestMethod]
        public void TransactionScopeFactory_Assure_Expected_Settings()
        {
            var expectedScope = TransactionScopeOption.RequiresNew;
            var searchFlags = BindingFlags.GetField | BindingFlags.NonPublic | BindingFlags.Instance;
            var appSettingsMock = new Mock<IAppSettings>();
            appSettingsMock.Setup(x=> x["TransactionScopeOption"]).Returns("RequiresNew");
            var factory = new TransactionScopeFactory(appSettingsMock.Object);
            var factoryType = typeof(TransactionScopeFactory);
            var scopeFieldInfo = factoryType.GetField("_scopeOption", searchFlags);

            var actualScope = (TransactionScopeOption)scopeFieldInfo.GetValue(factory);
            Assert.AreEqual(expectedScope, actualScope);
        }

        [TestMethod]
        public void TransactionScopeFactory_Assure_EmptyString_Settings_Converts_To_Default()
        {
            var expectedScope = TransactionScopeOption.Required;
            var searchFlags = BindingFlags.GetField | BindingFlags.NonPublic | BindingFlags.Instance;
            var appSettingsMock = new Mock<IAppSettings>();
            appSettingsMock.Setup(x=> x["TransactionScopeOption"]).Returns(string.Empty);
            var factory = new TransactionScopeFactory(appSettingsMock.Object);
            var factoryType = typeof(TransactionScopeFactory);
            var scopeFieldInfo = factoryType.GetField("_scopeOption", searchFlags);

            var actualScope = (TransactionScopeOption)scopeFieldInfo.GetValue(factory);
            Assert.AreEqual(expectedScope, actualScope);
        }



        [TestMethod]
        public void TransactionScopeFactory_Assure_Null_Settings_Converts_To_Default()
        {
            var expectedScope = TransactionScopeOption.Required;
            var searchFlags = BindingFlags.GetField | BindingFlags.NonPublic | BindingFlags.Instance;
            var appSettingsMock =  new Mock<IAppSettings>();
            appSettingsMock.Setup(x=> x["TransactionScopeOption"]).Returns(null as string);
            var factory = new TransactionScopeFactory(appSettingsMock.Object);
            var factoryType = typeof(TransactionScopeFactory);
            var scopeFieldInfo = factoryType.GetField("_scopeOption", searchFlags);

            var actualScope = (TransactionScopeOption)scopeFieldInfo.GetValue(factory);
            Assert.AreEqual(expectedScope, actualScope);
        }


        [TestMethod]
        public void TransactionScopeFactory_Assure_Garbage_Settings_Converts_To_Default()
        {
            var expectedScope = TransactionScopeOption.Required;
            var searchFlags = BindingFlags.GetField | BindingFlags.NonPublic | BindingFlags.Instance;
            var appSettingsMock = new Mock<IAppSettings>();
            appSettingsMock.Setup(x=> x["TransactionScopeOption"]).Returns("junk");
            var factory = new TransactionScopeFactory(appSettingsMock.Object);
            var factoryType = typeof(TransactionScopeFactory);
            var scopeFieldInfo = factoryType.GetField("_scopeOption", searchFlags);

            var actualScope = (TransactionScopeOption)scopeFieldInfo.GetValue(factory);
            Assert.AreEqual(expectedScope, actualScope);
        }



        [TestMethod]
        [Ignore]
        public void TransactionScopeFactory_GetTransactionScope_Returns_Transaction()
        {
            var appSettingsMock = new Mock<IAppSettings>();
            var factory = new TransactionScopeFactory(appSettingsMock.Object);
            var factoryType = typeof(TransactionScopeFactory);
            var actual = false;

            using (var trans = factory.GetTransactionScope())
            {
                actual = trans != null;
            }

            Assert.IsTrue(actual);
        }
    }
}
