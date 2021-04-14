using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace XlsxCompare
{
    class XlsxComparer
    {
        private readonly ILogger _logger;

        public XlsxComparer(ILogger<XlsxComparer> logger)
        {
            _logger = logger;
        }

        public CompareResult Compare(string leftPath, string rightPath, CompareOptions opts)
        {
            _logger.LogInformation("Comparing {LeftPath} to {RightPath}", leftPath, rightPath);
            using var left = XlsxFacade.Open(leftPath);
            using var right = XlsxFacade.Open(rightPath);

            var mismatches = Compare(left, right, opts).ToList();

            _logger.LogInformation("Compared");

            return new CompareResult(mismatches);
        }

        private IEnumerable<Mismatch> Compare(XlsxFacade left, XlsxFacade right, CompareOptions opts)
        {
            _logger.LogInformation("Comparing {LeftXlsx} to {RightXlsx}", left, right);
            foreach (var leftRow in left.Rows)
            {
                var key = left.GetSafeValue(leftRow, opts.LeftKeyColumn);
                if (!right.TryFindRow(opts.RightKeyColumn, key, out var rightRow))
                {
                    if (opts.IgnoreMissingRows) { continue; }
                    throw new KeyNotFoundException($"Could not find '{key}' in {opts.RightKeyColumn}");
                }

                foreach (var assertion in opts.Assertions)
                {
                    var leftValue = left.GetSafeValue(leftRow, assertion.LeftColumnName);
                    var rightValue = right.GetSafeValue(rightRow, assertion.RightColumnName);

                    if (!assertion.IsMatch(leftValue, rightValue))
                    {
                        var leftContext = GetContext(left, leftRow, opts.ResultOptions.LeftColumnNames);
                        var rightContext = GetContext(right, rightRow, opts.ResultOptions.RightColumnNames);
                        var context = new Dictionary<string, string>(
                            leftContext.Concat(rightContext));
                        yield return new Mismatch(assertion, key, leftValue, rightValue, context);
                    }
                }
            }
        }

        private static IEnumerable<KeyValuePair<string, string>> GetContext(XlsxFacade xlsx, int row, IReadOnlyCollection<string>? columnNames)
        {
            if (columnNames == null) { yield break; }
            foreach (var colName in columnNames)
            {
                yield return KeyValuePair.Create(
                    colName,
                    xlsx.GetSafeValue(row, colName));
            }
        }
    }
}
