using System.Collections.Generic;

namespace XlsxCompare
{
    record Mismatch(
        Assertion Assertion,
        string Key,
        string LeftValue,
        string RightValue,
        IReadOnlyDictionary<string, string> Context
    );
}
