using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static XlsxCompare.Tests.Helpers;

namespace XlsxCompare.Tests
{
    [TestClass]
    public class XlsxComparerTests
    {
        private readonly XlsxComparer _comparer = new(CreateLogger<XlsxComparer>());

        [TestMethod]
        public void Compare_NoErrors_ReturnsCorrectResult()
        {
            var opts = new CompareOptions(
                LeftKeyColumn: "id",
                RightKeyColumn: "old_id",
                new[]{
                    new Assertion("Address", "ADDR"),
                    new Assertion("Email", "EML")
                },
                new ResultOptions()
            );
            var results = _comparer.Compare("left.xlsx", "right.xlsx", opts);

            Assert.AreEqual(0, results.TotalCellMismatches);
            Assert.AreEqual(0, results.TotalRowMismatches);
        }

        [TestMethod]
        public void Compare_SomeErrors_ReturnsCorrectResult()
        {
            var opts = new CompareOptions(
                LeftKeyColumn: "id",
                RightKeyColumn: "old_id",
                new[]{
                    new Assertion("Name", "CUSTOMER_NAME"),
                    new Assertion("Added", "EXTRA")
                },
                new ResultOptions(
                    LeftColumnNames: new[] { "id" },
                    RightColumnNames: new[] { "new_id" }
                )
            );
            var results = _comparer.Compare("left.xlsx", "right.xlsx", opts);

            Assert.AreEqual(5, results.TotalCellMismatches);
            Assert.AreEqual(3, results.TotalRowMismatches);


            var expectedContext = new Dictionary<string, string>(){
                {"1", "1000"},
                {"2", "1002"},
                {"3", "1001"}
            };
            foreach (var mismatch in results.Mismatches)
            {
                var id = mismatch.Context["id"];
                var newId = mismatch.Context["new_id"];
                Assert.AreEqual(expectedContext[id], newId);
            }
        }

        [TestMethod]
        public void Compare_NoJoinMatchesWithContext_ReturnsMissingDataAsMismatch()
        {
            var opts = new CompareOptions(
                LeftKeyColumn: "id",
                RightKeyColumn: "new_id",
                new[]{
                    new Assertion("Email", "EML")
                },
                new ResultOptions(
                    LeftColumnNames: new[] { "Name" }
                )
            );
            var results = _comparer.Compare("left.xlsx", "right.xlsx", opts);

            Assert.AreEqual(3, results.TotalCellMismatches);
            Assert.AreEqual(3, results.TotalRowMismatches);

            var expectedContext = new Dictionary<string, string>(){
                {"1", "leading trailing spaces"},
                {"2", "null email"},
                {"3", "whitespace address"}
            };

            foreach (var mismatch in results.Mismatches)
            {
                var id = mismatch.Key;
                var name = mismatch.Context["Name"];
                Assert.AreEqual(expectedContext[id], name);
            }
        }

        [TestMethod]
        public void Compare_NoJoinMatchesWithIgnoringMissingRows_ReturnsNoMismatches()
        {
            var opts = new CompareOptions(
                LeftKeyColumn: "id",
                RightKeyColumn: "new_id",
                Assertions: new[]{
                    new Assertion("Email", "EML")
                },
                ResultOptions: new ResultOptions(),
                IgnoreMissingRows: true
            );
            var results = _comparer.Compare("left.xlsx", "right.xlsx", opts);

            Assert.AreEqual(0, results.TotalCellMismatches);
            Assert.AreEqual(0, results.TotalRowMismatches);
        }
    }
}
