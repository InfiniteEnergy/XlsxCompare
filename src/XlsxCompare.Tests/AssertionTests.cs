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
            yield return new object[]{
                new Assertion("leftCol", "rightCol", ZeroRepresentsEmpty: true),
                "0",
                ""
            };
            yield return new object[]{
                new Assertion("leftCol", "rightCol", ZeroRepresentsEmpty: true),
                "",
                "0.00"
            };
            yield return new object[]{
                new Assertion("leftCol", "rightCol",
                    MatchBy: MatchBy.Tokens,
                    Synonyms: new[]{new HashSet<string>(new[]{"rd", "Road"})}),
                "1234\tFOO\tROAD\tS\t# 123A",
                "1234 S Foo Rd # 123A"
            };

            yield return new object[]{
                new Assertion("leftCol", "rightCol",
                    Synonyms: new[]{new HashSet<string>(new[]{"suite", " ste"})}),
                "Suite 2",
                "Ste 2"
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
            yield return new object[]{
                new Assertion("leftCol", "rightCol", ZeroRepresentsEmpty: true),
                "",
                "0.001"
            };
            yield return new object[]{
                new Assertion("leftCol", "rightCol", ZeroRepresentsEmpty: true),
                "1",
                "0"
            };
            yield return new object[]{
                new Assertion("leftCol", "rightCol", Synonyms: new[]{new HashSet<string>(new[]{"rd", "road"})}),
                "foord",
                "fooroad"
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
