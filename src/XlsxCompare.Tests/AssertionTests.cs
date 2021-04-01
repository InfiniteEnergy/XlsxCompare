using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace XlsxCompare.Tests
{
    [TestClass]
    public class AssertionTests
    {

        public static IEnumerable<object[]> AssertionsThatMatch()
        {
            yield return new object[]{
                new Assertion("leftCol", "rightCol"),
                "a",
                "a"
            };
            yield return new object[]{
                new Assertion("leftCol", "rightCol", Remove: "asdf"),
                "a asdf",
                "asdf a"
            };
        }

        [TestMethod]
        [DynamicData(nameof(AssertionsThatMatch), DynamicDataSourceType.Method)]
        public void IsMatch_ThingsThatMatch_ReturnsTrue(Assertion assertion, string left, string right)
        {
            Assert.IsTrue(assertion.IsMatch(left, right));
        }

        public static IEnumerable<object[]> AssertionsThatDoNotMatch()
        {
            yield return new object[]{
                new Assertion("leftCol", "rightCol"),
                "0",
                ""
            };
            yield return new object[]{
                new Assertion("leftCol", "rightCol", Remove: "asdf"),
                "a asdf",
                "asdf b"
            };
        }
        [TestMethod]
        [DynamicData(nameof(AssertionsThatDoNotMatch), DynamicDataSourceType.Method)]
        public void IsMatch_ThingsThatDoNotMatch_ReturnsFalse(Assertion assertion, string left, string right)
        {
            Assert.IsFalse(assertion.IsMatch(left, right));
        }




    }
}
