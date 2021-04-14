using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace XlsxCompare.Tests
{
    [TestClass]
    public class XlsxFacadeTests
    {

        [TestMethod]
        [DataRow("left.xlsx", "Sheet1", "A1:E4")]
        [DataRow("right.xlsx", "DATA DUMP", "A1:G4")]
        public void ToString_Various_ReflectsSheetAndSize(string path, string sheet, string range)
        {
            using var xlsx = XlsxFacade.Open(path);

            StringAssert.Contains(xlsx.ToString(), sheet);
            StringAssert.Contains(xlsx.ToString(), range);
        }

        [TestMethod]
        [DataRow(2, "Id", "1")]
        [DataRow(2, "ID", "1")]
        [DataRow(2, "address", "test")]
        [DataRow(3, "email", "")]
        [DataRow(4, "address", "")]
        [DataRow(4, "added", "")]
        public void GetSafeValue_Various_ReturnsUsefulValue(int row, string column, string expected)
        {
            using var xlsx = XlsxFacade.Open("left.xlsx");

            var actual = xlsx.GetSafeValue(row, column);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        [DataRow("Id", "3", 4)]
        [DataRow("email", "foo@example.com", 2)]
        [DataRow("address", "test", 2)]
        public void TryFindRow_Exists_ReturnsTrueAndRowIndex(string columnName, string value, int expected)
        {
            using var xlsx = XlsxFacade.Open("left.xlsx");

            var wasFound = xlsx.TryFindRow(columnName, value, out var actual);

            Assert.IsTrue(wasFound);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TryFindRow_DoesNotExist_ReturnsFalse()
        {
            using var xlsx = XlsxFacade.Open("left.xlsx");
            var wasFound = xlsx.TryFindRow("id", "-1", out var actual);
            Assert.IsFalse(wasFound);
        }

        [TestMethod]
        public void Rows_ReturnIndexesWithoutHeader()
        {
            using var xlsx = XlsxFacade.Open("left.xlsx");

            var actual = xlsx.Rows.ToArray();

            CollectionAssert.AreEqual(
                new[] { 2, 3, 4 },
                actual);
        }
    }
}
