using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestHelpers
{
    public static class AssertExtentions
    {
        public static T ShouldThrow<T>(this Action methodUnderTest) where T : Exception
        {
            try
            {
                methodUnderTest();
            }
            catch (T expected)
            {
                // success
                return expected;
            }
            catch (Exception unexpected)
            {
                Assert.Fail($"An exception of type {typeof(T).Name} was expected to be thrown, but {unexpected.GetType().Name} was thrown instead.", unexpected);
                return default(T);
            }
            Assert.Fail($"An exception of type {typeof(T).Name} was expected to be thrown.");
            return default(T);
        }

        public static async Task<T> ShouldThrow<T>(this Func<Task> methodUnderTest) where T : Exception
        {
            try
            {
                await methodUnderTest();
            }
            catch (T expected)
            {
                // success
                return expected;
            }
            catch (Exception unexpected)
            {
                Assert.Fail($"An exception of type {typeof(T).Name} was expected to be thrown, but {unexpected.GetType().Name} was thrown instead.", unexpected);
                return default(T);
            }
            Assert.Fail($"An exception of type {typeof(T).Name} was expected to be thrown.");
            return default(T);
        }


    }
}
