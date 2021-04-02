namespace XlsxCompare
{
    public record Assertion(
        string LeftColumnName,
        string RightColumnName,
        MatchBy? MatchBy = null,
        string? Remove = null)
    {
        public bool IsMatch(string left, string right)
        {
            if (Remove != null)
            {
                left = left.Replace(Remove, "").Trim();
                right = right.Replace(Remove, "").Trim();
            }
            return MatchBy.IsMatch(left, right);
        }
    };
}
