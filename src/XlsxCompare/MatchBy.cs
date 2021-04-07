using System;

namespace XlsxCompare
{
    public enum MatchBy
    {
        None = 0,
        String,
        StringIgnoreMissingLeft,
        Integer,
        Decimal,
        Date,
        StringLeftStartsWithRight,
        ZeroRepresentsEmpty,
        DecimalZeroRepresentsEmpty,
    }

    static class MatchByExtensions
    {
        public static bool IsMatch(this MatchBy? match, string left, string right)
            => match switch
            {
                MatchBy.ZeroRepresentsEmpty => IsStringMatch(NormalizeZeroToEmpty(left), NormalizeZeroToEmpty(right)),
                MatchBy.DecimalZeroRepresentsEmpty => IsDecimalMatch(NormalizeZeroToEmpty(left), NormalizeZeroToEmpty(right)),
                MatchBy.Date => IsDateMatch(left, right),
                MatchBy.Integer => IsIntegerMatch(left, right),
                MatchBy.StringIgnoreMissingLeft => left.Length == 0 || IsStringMatch(left, right),
                MatchBy.StringLeftStartsWithRight => IsLeftStartsWithRightMatch(left, right),
                MatchBy.Decimal => IsDecimalMatch(left, right),
                _ => IsStringMatch(left, right),
            };

        private static bool IsStringMatch(string left, string right)
            => left.Equals(right, StringComparison.OrdinalIgnoreCase);

        private static bool IsIntegerMatch(string left, string right)
            => IsStringMatch(left, right)
                || (int.TryParse(left, out var leftInt)
                    && int.TryParse(right, out var rightInt)
                    && leftInt == rightInt);

        private static bool IsDateMatch(string left, string right)
            => IsStringMatch(left, right)
                || (DateTime.TryParse(left, out var leftDate)
                    && DateTime.TryParse(right, out var rightDate)
                    && leftDate.Date == rightDate.Date);

        private static bool IsLeftStartsWithRightMatch(string left, string right)
            => right.Length > 0
                && left.StartsWith(right, StringComparison.OrdinalIgnoreCase);

        private static string NormalizeZeroToEmpty(string input)
            => decimal.TryParse(input, out var parsed) && parsed == 0
                ? ""
                : input;

        private static bool IsDecimalMatch(string left, string right)
            => IsStringMatch(left, right)
                || (decimal.TryParse(left, out var leftInt)
                    && decimal.TryParse(right, out var rightInt)
                    && leftInt == rightInt);
    }
}
