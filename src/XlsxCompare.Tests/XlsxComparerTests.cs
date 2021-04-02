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
                }
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
                }
            );
            var results = _comparer.Compare("left.xlsx", "right.xlsx", opts);

            Assert.AreEqual(5, results.TotalCellMismatches);
            Assert.AreEqual(3, results.TotalRowMismatches);
        }
    }
}
