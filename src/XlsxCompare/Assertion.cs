using System;
using System.Collections.Generic;
using System.Linq;

namespace XlsxCompare
{

    public record Assertion(
        string LeftColumnName,
        string RightColumnName,
        MatchBy? MatchBy = null,
        string? Remove = null,
        bool ZeroRepresentsEmpty = false,
        IReadOnlyCollection<ISet<string>>? Synonyms = null
        )
    {
        private bool HasSynonyms => Synonyms?.Any() == true;

        /// <summary>
        /// map from a synonym to it's canonical form
        /// </summary>
        private IReadOnlyDictionary<string, string> SynonymMap
            => _synonymMapCache ??= CreateSynonymMap();
        private IReadOnlyDictionary<string, string>? _synonymMapCache;

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
            if (HasSynonyms)
            {
                left = NormalizeSynonyms(left);
                right = NormalizeSynonyms(right);
            }

            return MatchBy.IsMatch(left, right);
        }

        private static string NormalizeZeroToEmpty(string input)
            => decimal.TryParse(input, out var parsed) && parsed == 0
                ? ""
                : input;

        private string NormalizeSynonyms(string input)
        {
            var tokens = input.Split(new[] { ' ', '\t', '\n', '\r' }, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
                .Select(NormalizeSynonym);
            return string.Join(' ', tokens);
        }

        private string NormalizeSynonym(string token)
            => SynonymMap.TryGetValue(token, out var replacement)
                    ? replacement
                    : token;

        private IReadOnlyDictionary<string, string> CreateSynonymMap()
        {
            // don't want to care about capitalization
            var mapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            foreach (var set in Synonyms ?? Enumerable.Empty<ISet<string>>())
            {
                // don't want to care about whitespace
                var winner = set.First().Trim();
                foreach (var synonym in set.Select(x => x.Trim()))
                {
                    if (synonym.Contains(' '))
                    {
                        throw new NotSupportedException($"multi-word synonyms are not supported: '{synonym}'");
                    }
                    mapping.Add(synonym, winner);
                };
            }

            return mapping;
        }

    }
}
