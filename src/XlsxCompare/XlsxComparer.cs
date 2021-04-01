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
                var rightRow = right.FindRow(opts.RightKeyColumn, key);

                foreach (var assertion in opts.Assertions)
                {
                    var leftValue = left.GetSafeValue(leftRow, assertion.LeftColumnName);
                    var rightValue = right.GetSafeValue(rightRow, assertion.RightColumnName);

                    if (!assertion.IsMatch(leftValue, rightValue))
                    {
                        yield return new Mismatch(assertion, key, leftValue, rightValue);
                    }
                }
            }
        }

    }


}
