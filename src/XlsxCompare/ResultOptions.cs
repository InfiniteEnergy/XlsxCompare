using System.Collections.Generic;

namespace XlsxCompare
{
    record ResultOptions(
        string LeftValueHeader = "left value",
        string RightValueHeader = "right value",
        IReadOnlyCollection<string>? LeftColumnNames = null,
        IReadOnlyCollection<string>? RightColumnNames = null
    );
}

