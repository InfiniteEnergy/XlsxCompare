namespace XlsxCompare
{
    record Mismatch(Assertion Assertion, string Key, string LeftValue, string RightValue);
}
