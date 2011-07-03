using LinqToTwitterTests.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LinqToTwitterTests
{
    /// <summary>
    ///This is a test class for ExpressionTreeHelpersTest and is intended
    ///to contain all ExpressionTreeHelpersTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ExpressionTreeHelpersTest
    {
        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
            TestCulture.SetCulture();
        }

        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion

        // TODO: Not yet implemented

        /// <summary>
        ///A test for IsSpecificMemberExpression
        ///</summary>
        [TestMethod()]
        public void IsSpecificMemberExpressionTest()
        {
            //TODO: Pass the Expression ConvertChecked(t.Type)
            ////Expression<Func<Status, bool>> exp = t => ConvertChecked(t.Type) == 4;
            //Status t = new Status();
            //Expression<Func<StatusType>> stType = () => t.Type;
            //UnaryExpression ue = Expression.MakeUnary(ExpressionType.ConvertChecked, stType, typeof(Func<StatusType>));
            ////Expression<Func<StatusType>> stVal = () => StatusType.Mentions;
            ////BinaryExpression be = Expression.MakeBinary(ExpressionType.Equal, ue, stVal);
            //Type declaringType = typeof(Status);
            //string memberName = "Type";
            //bool expected = true;
            //bool actual = ExpressionTreeHelpers.IsSpecificMemberExpression(ue, declaringType, memberName);
            //Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for IsMemberEqualsValueExpression
        ///</summary>
        [TestMethod()]
        public void IsMemberEqualsValueExpressionTest()
        {
            //Expression exp = null; // TODO: Initialize to an appropriate value
            //Type declaringType = null; // TODO: Initialize to an appropriate value
            //string memberName = string.Empty; // TODO: Initialize to an appropriate value
            //bool expected = false; // TODO: Initialize to an appropriate value
            //bool actual;
            //actual = ExpressionTreeHelpers.IsMemberEqualsValueExpression(exp, declaringType, memberName);
            //Assert.AreEqual(expected, actual);
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetValueFromExpression
        ///</summary>
        [TestMethod()]
        public void GetValueFromExpressionTest()
        {
            //Expression expression = null; // TODO: Initialize to an appropriate value
            //string expected = string.Empty; // TODO: Initialize to an appropriate value
            //string actual;
            //actual = ExpressionTreeHelpers.GetValueFromExpression(expression);
            //Assert.AreEqual(expected, actual);
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetValueFromEqualsExpression
        ///</summary>
        [TestMethod()]
        public void GetValueFromEqualsExpressionTest()
        {
            //BinaryExpression be = null; // TODO: Initialize to an appropriate value
            //Type memberDeclaringType = null; // TODO: Initialize to an appropriate value
            //string memberName = string.Empty; // TODO: Initialize to an appropriate value
            //string expected = string.Empty; // TODO: Initialize to an appropriate value
            //string actual;
            //actual = ExpressionTreeHelpers.GetValueFromEqualsExpression(be, memberDeclaringType, memberName);
            //Assert.AreEqual(expected, actual);
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }
    }
}
