using System.Collections.Generic;
using System.Linq;

namespace XlsxCompare
{
    record CompareResult(IReadOnlyCollection<Mismatch> Mismatches)
    {
        public int TotalCellMismatches => Mismatches.Count;
        public int TotalRowMismatches => Mismatches.Select(x => x.Key).Distinct().Count();
    }
}
