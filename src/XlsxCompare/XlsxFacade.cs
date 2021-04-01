using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using OfficeOpenXml;

namespace XlsxCompare
{

    /// <summary>
    /// wraps <see cref="ExcelPackage"/>
    /// </summary>
    sealed class XlsxFacade : IDisposable
    {
        readonly ExcelPackage _excel;
        readonly IReadOnlyDictionary<string, int> _columnMap;

        private ExcelWorksheet Sheet => _excel.Workbook.Worksheets[0];
        public IEnumerable<int> Rows => Enumerable.Range(2, Sheet.Dimension.Rows - 1);

        private XlsxFacade(FileInfo file)
        {
            _excel = new ExcelPackage(file);
            _columnMap = BuildColumnMap(Sheet);
        }

        /// <summary>
        /// converts everything to trimmed strings, normalizing `null`s, "NULL"s,
        /// and "" to ""
        /// </summary>
        public string GetSafeValue(int row, string columnName)
        {
            var col = _columnMap[columnName];
            var value = Sheet.Cells[row, col].Value;
            return Normalize(value?.ToString());
        }

        private static string Normalize(string? value)
            => value switch
            {
                "NULL" => "",
                null => "",
                _ => value.Trim()
            };

        public int FindRow(string columnName, string value)
        {
            foreach (var row in Rows)
            {
                var candidate = GetSafeValue(row, columnName);
                if (value.Equals(candidate, StringComparison.OrdinalIgnoreCase))
                {
                    return row;
                }
            }
            throw new KeyNotFoundException($"Could not find '{value}' in {columnName}");
        }

        public void Dispose() => _excel.Dispose();

        public override string ToString()
        {
            return $"[{GetType().Name} {Sheet.Name}, {Sheet.Dimension}]";
        }

        public static XlsxFacade Open(string path)
        {
            var file = new FileInfo(path);
            if (!file.Exists) { throw new FileNotFoundException("Could not open xlsx", file.FullName); }
            return new XlsxFacade(file);
        }

        static IReadOnlyDictionary<string, int> BuildColumnMap(ExcelWorksheet sheet)
            => Enumerable.Range(1, sheet.Dimension.Columns)
                .Select(index => new
                {
                    index,
                    header = sheet.Cells[1, index].Value?.ToString()
                })
                .Where(x => x.header != null)
                .ToDictionary(
                    x => x.header!.Trim(),
                    x => x.index,
                    StringComparer.OrdinalIgnoreCase);
    }
}
