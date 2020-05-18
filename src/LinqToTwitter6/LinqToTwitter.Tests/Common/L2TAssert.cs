using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LinqToTwitter.Tests.Common
{
    class L2TAssert
    {
        public static async Task<T> Throws<T>(Func<Task> task) 
            where T : Exception
        {
            try
            {
                await task();
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(T));
                return (T)ex;
            }

            if (typeof(T).Equals(new Exception().GetType()))
                Assert.Fail("Expected exception but no exception was thrown.");
            else
                Assert.Fail(
                    string.Format(
                        "Expected exception of type {0} but no exception was thrown.", 
                        typeof(T)));

            return null;
        }

        public static T Throws<T>(Action task)
            where T : Exception
        {
            try
            {
                task();
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(T));
                return (T)ex;
            }

            if (typeof(T).Equals(new Exception().GetType()))
                Assert.Fail("Expected exception but no exception was thrown.");
            else
                Assert.Fail(
                    string.Format(
                        "Expected exception of type {0} but no exception was thrown.",
                        typeof(T)));

            return null;
        }
    }
}
