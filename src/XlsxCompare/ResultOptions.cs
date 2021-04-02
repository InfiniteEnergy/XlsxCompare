using System;
using System.Collections.Generic;
using System.Linq;

namespace XlsxCompare
{
    record ResultOptions(
        string Path = "results.xlsx",
        string LeftValueHeader = "left value",
        string RightValueHeader = "right value",
        IReadOnlyCollection<string>? LeftColumnNames = null,
        IReadOnlyCollection<string>? RightColumnNames = null
    )
    {
        internal IEnumerable<string> ContextColumnNames
        {
            get
            {
                var left = LeftColumnNames ?? Enumerable.Empty<string>();
                var right = RightColumnNames ?? Enumerable.Empty<string>();
                return left.Concat(right);
            }
        }
    };
}

