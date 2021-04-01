namespace XlsxCompare
{
    public record Assertion(
        string LeftColumnName,
        string RightColumnName,
        MatchBy? MatchBy = null,
        string? Remove = null);

    static class AssertionExtensions
    {
        public static bool IsMatch(this Assertion assertion, string left, string right)
        {
            if (assertion.Remove != null)
            {
                left = left.Replace(assertion.Remove, "").Trim();
                right = right.Replace(assertion.Remove, "").Trim();
            }
            return assertion.MatchBy.IsMatch(left, right);
        }
    }

}
