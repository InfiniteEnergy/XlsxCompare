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

        public bool TryFindRow(string columnName, string value, out int foundRow)
        {
            foundRow = -1;
            foreach (var row in Rows)
            {
                var candidate = GetSafeValue(row, columnName);
                if (value.Equals(candidate, StringComparison.OrdinalIgnoreCase))
                {
                    foundRow = row;
                    return true;
                }
            }
            return false;
        }

        public void Dispose() => _excel.Dispose();

        public override string ToString()
        {
            return $"[{GetType().Name} {Sheet.Name}, {Sheet.Dimension}]";
        }

        public static XlsxFacade Open(string path)
        {
            var file = new FileInfo(path);
            if (!".xlsx".Equals(file.Extension, StringComparison.OrdinalIgnoreCase))
            {
                throw new NotSupportedException($"Cannot process {path}, only xlsx files are supported");
            }
            if (!file.Exists) { throw new FileNotFoundException("Could not open xlsx", file.FullName); }
            return new XlsxFacade(file);
        }

        IReadOnlyDictionary<string, int> BuildColumnMap(ExcelWorksheet sheet)
        {
            var result = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            var headers = Enumerable.Range(1, sheet.Dimension.Columns)
                .Select(index => new
                {
                    index,
                    name = sheet.Cells[1, index].Value?.ToString()?.Trim()
                })
                .Where(x => x.name != null);

            foreach (var header in headers)
            {
                if (result.ContainsKey(header.name!))
                {
                    throw new ArgumentException($"Failed to open {_excel.File}, column headers must be unique. {_excel.File} contains duplicate column header '{header.name}'.");
                }
                result.Add(header.name!, header.index);
            }
            return result;
        }
    }
}
