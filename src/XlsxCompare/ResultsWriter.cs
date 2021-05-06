using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;

namespace XlsxCompare
{
    class ResultsWriter
    {
        private readonly ILogger _logger;

        public ResultsWriter(ILogger<ResultsWriter> logger)
        {
            _logger = logger;
        }

        public void Write(CompareResult results, ResultOptions opts)
        {
            _logger.LogInformation("Writing results to {Path}", opts.Path);

            try
            {
                File.Delete(opts.Path);
            }
            catch (IOException ioe) when (ioe.Message.Contains("used by another process", StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogWarning("Unable to write to {Path}, trying again with a unique suffix", opts.Path);
                var newOpts = opts with
                {
                    Path = opts.Path.Replace(".xlsx", $"_{DateTime.Now:yyyyMMdd'T'HHmm}.xlsx")
                };
                Write(results, newOpts);
                return;
            }

            using var xlsx = new ExcelPackage(new FileInfo(opts.Path));
            ExcelWorksheet sheet = xlsx.Workbook.Worksheets.Add("Sheet1")
                ?? throw new InvalidOperationException("unable to add a sheet");

            // add the header row
            var row = 1;
            var headers = GetHeaders(opts).ToList();
            WriteRow(sheet, row++, headers);

            // add the data
            foreach (var mismatch in results.Mismatches.OrderBy(x => x.Key))
            {
                WriteRow(sheet, row++, GetValues(opts, mismatch));
            }

            var headerRange = sheet.Cells[1, 1, 1, headers.Count];
            headerRange.Style.Font.Bold = true;
            headerRange.AutoFilter = true;
            foreach (var col in Enumerable.Range(1, headers.Count))
            {
                sheet.Column(col).AutoFit();
            }
            xlsx.Save();
            _logger.LogInformation("Written");
        }

        private static IEnumerable<string> GetHeaders(ResultOptions opts)
        {
            foreach (var col in opts.ContextColumnNames)
            {
                yield return col;
            }
            yield return "Mismatched field";
            yield return opts.LeftValueHeader;
            yield return opts.RightValueHeader;
        }

        private static IEnumerable<string?> GetValues(ResultOptions opts, Mismatch mismatch)
        {
            foreach (var col in opts.ContextColumnNames)
            {
                yield return mismatch.Context.GetValueOrDefault(col);
            }
            yield return mismatch.Assertion.LeftColumnName;
            // TODO: maybe format?
            yield return mismatch.LeftValue;
            yield return mismatch.RightValue;
        }

        private static void WriteRow(ExcelWorksheet sheet, int row, IEnumerable<string?> values)
        {
            var col = 1;
            foreach (var value in values)
            {
                sheet.SetValue(row, col++, value);
            }
        }
    }
}

