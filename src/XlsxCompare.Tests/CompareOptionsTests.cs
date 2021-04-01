using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace XlsxCompare.Tests
{
    [TestClass]
    public class CompareOptionsTests
    {

        [TestMethod]
        [DataRow(@"{}", nameof(CompareOptions.LeftKeyColumn))]
        [DataRow(@"{'LeftKeyColumn': ''}", nameof(CompareOptions.LeftKeyColumn))]
        [DataRow(@"{'LeftKeyColumn': '  '}", nameof(CompareOptions.LeftKeyColumn))]
        [DataRow(@"{'LeftKeyColumn': 'ok'}", nameof(CompareOptions.RightKeyColumn))]
        [DataRow(@"{'LeftKeyColumn': 'ok', 'RightKeyColumn': 'ok'}", nameof(CompareOptions.Assertions))]
        [DataRow(@"{'LeftKeyColumn': 'ok', 'RightKeyColumn': 'ok', 'Assertions':[]}", nameof(CompareOptions.Assertions))]
        [DataRow(@"{'LeftKeyColumn': 'ok', 'RightKeyColumn': 'ok', 'Assertions':[{}]}", nameof(Assertion.LeftColumnName))]
        [DataRow(@"{'LeftKeyColumn': 'ok', 'RightKeyColumn': 'ok', 'Assertions':[{'LeftColumnName': 'ok'}]}", nameof(Assertion.RightColumnName))]
        public void FromJson_MissingData_Throws(string json, string missingField)
        {
            var ex = Assert.ThrowsException<InvalidOperationException>(
                () => CompareOptions.FromJson(json.Replace('\'', '"'))
            );

            StringAssert.Contains(ex.Message, missingField);
        }

        [TestMethod]
        public void FromJson_ValidData_Parses()
        {
            var expected = new CompareOptions("leftKey", "rightKey", new[]{
                new Assertion("leftCol", "rightCol", null),
                new Assertion("leftCol2", "rightCol2", MatchBy.None)
            });

            var json = @"{
                'leftKeyColumn': 'leftKey',
                'rightKeyColumn': 'rightKey',
                'assertions':[
                    {'LeftColumnName': 'leftCol', 'RightColumnName': 'rightCol'},
                    {'leftColumnName': 'leftCol2', 'rightColumnName': 'rightCol2', 'matchBy': 'none'}
                ]
            }".Replace('\'', '"');

            var actual = CompareOptions.FromJson(json);
            Assert.AreEqual(expected.LeftKeyColumn, actual.LeftKeyColumn);
            Assert.AreEqual(expected.RightKeyColumn, actual.RightKeyColumn);
            CollectionAssert.AreEqual(
                expected.Assertions.ToArray(),
                actual.Assertions.ToArray()
            );
        }
    }
}
