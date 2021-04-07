namespace XlsxCompare
{
    public record Assertion(
        string LeftColumnName,
        string RightColumnName,
        MatchBy? MatchBy = null,
        string? Remove = null,
        bool ZeroRepresentsEmpty = false
        )
    {
        public bool IsMatch(string left, string right)
        {
            if (Remove != null)
            {
                left = left.Replace(Remove, "").Trim();
                right = right.Replace(Remove, "").Trim();
            }
            if (ZeroRepresentsEmpty)
            {
                left = NormalizeZeroToEmpty(left);
                right = NormalizeZeroToEmpty(right);
            }
            return MatchBy.IsMatch(left, right);
        }

        private static string NormalizeZeroToEmpty(string input)
            => decimal.TryParse(input, out var parsed) && parsed == 0
                ? ""
                : input;
    };
}
