using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace XlsxCompare.Tests
{
    [TestClass]
    public class MatchByTests
    {

        [TestMethod]
        [DataRow(null, "a", "a")]
        [DataRow(MatchBy.String, "A", "a")]
        [DataRow(MatchBy.String, "", "")]
        [DataRow(MatchBy.StringIgnoreMissingLeft, "", "asdf")]
        [DataRow(MatchBy.Integer, "123", "0123")]
        [DataRow(MatchBy.Integer, "", "")]
        [DataRow(MatchBy.Date, "", "")]
        [DataRow(MatchBy.Date, "2021-04-01", "04/01/2021")]
        [DataRow(MatchBy.Date, "2021-04-01 4:00AM", "04/01/2021")]
        [DataRow(MatchBy.StringLeftStartsWithRight, "asdf", "as")]
        public void IsMatch_ThingsThatMatch_ReturnsTrue(MatchBy? match, string left, string right)
        {
            Assert.IsTrue(match.IsMatch(left, right));
        }

        [TestMethod]
        [DataRow(null, "a", "b")]
        [DataRow(MatchBy.String, "a", "b")]
        [DataRow(MatchBy.StringIgnoreMissingLeft, "a", "asdf")]
        [DataRow(MatchBy.Integer, "123", "124")]
        [DataRow(MatchBy.Date, "2021-04-01", "04/02/2021")]
        [DataRow(MatchBy.StringLeftStartsWithRight, "as", "asdf")]
        [DataRow(MatchBy.StringLeftStartsWithRight, "asdf", "")]
        public void IsMatch_ThingsThatDoNotMatch_ReturnsFalse(MatchBy? match, string left, string right)
        {
            Assert.IsFalse(match.IsMatch(left, right));
        }
    }
}
