using System;
using System.Collections.Generic;
using System.Linq;

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
        StringRightStartsWithLeft,
        Tokens,
    }

    static class MatchByExtensions
    {
        public static bool IsMatch(this MatchBy? match, string left, string right)
            => match switch
            {
                MatchBy.Date => IsDateMatch(left, right),
                MatchBy.Integer => IsIntegerMatch(left, right),
                MatchBy.StringIgnoreMissingLeft => left.Length == 0 || IsStringMatch(left, right),
                MatchBy.StringLeftStartsWithRight => IsLeftStartsWithRightMatch(left, right),
                MatchBy.StringRightStartsWithLeft => IsLeftStartsWithRightMatch(right, left),
                MatchBy.Decimal => IsDecimalMatch(left, right),
                MatchBy.Tokens => IsTokenMatch(left, right),
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
            => (left, right) switch
            {
                ("", "") => true, // both empty is a match
                (_, "") => false, // left starting with empty doesn't count
                (_, _) => left.StartsWith(right, StringComparison.OrdinalIgnoreCase),
            };

        private static bool IsDecimalMatch(string left, string right)
            => IsStringMatch(left, right)
                || (decimal.TryParse(left, out var leftInt)
                    && decimal.TryParse(right, out var rightInt)
                    && leftInt == rightInt);

        private static bool IsTokenMatch(string left, string right)
            => IsTokenMatch(Tokenize(left), Tokenize(right));

        private static bool IsTokenMatch(IEnumerable<string> left, IEnumerable<string> right)
            => !left.Except(right, StringComparer.OrdinalIgnoreCase).Any();

        private static IEnumerable<string> Tokenize(string value)
            => value.Split(' ').Select(x => x.Trim());
    }
}
