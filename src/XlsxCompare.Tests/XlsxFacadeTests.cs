using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace XlsxCompare.Tests
{
    [TestClass]
    public class XlsxFacadeTests
    {

        [TestMethod]
        [DataRow("left.xlsx", "Sheet1", "A1:E4")]
        [DataRow("right.xlsx", "DATA DUMP", "A1:F3")]
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
        public void FindRow_Exists_ReturnsRowIndex(string columnName, string value, int expected)
        {
            using var xlsx = XlsxFacade.Open("left.xlsx");

            var actual = xlsx.FindRow(columnName, value);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public void FindRow_DoesNotExist_Throws()
        {
            using var xlsx = XlsxFacade.Open("left.xlsx");
            xlsx.FindRow("id", "-1");
        }
    }
}
