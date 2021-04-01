namespace XlsxCompare
{
    record Assertion(string LeftColumnName, string RightColumnName, MatchBy? MatchBy);
}
