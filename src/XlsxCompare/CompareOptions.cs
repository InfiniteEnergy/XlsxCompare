using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace XlsxCompare
{
    record CompareOptions(string LeftKeyColumn, string RightKeyColumn, IReadOnlyCollection<Assertion> Assertions)
    {
        public static CompareOptions FromJsonFile(string path) => FromJson(File.ReadAllText(path));

        public static CompareOptions FromJson(string json)
        {
            var jsonOpts = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                AllowTrailingCommas = true,
            };
            jsonOpts.Converters.Add(new JsonStringEnumConverter());

            var opts = JsonSerializer.Deserialize<CompareOptions>(json, jsonOpts)
                ?? throw new InvalidOperationException("failed");

            // `Deserialize` doesn't respect non-null strings, validate
            if (string.IsNullOrWhiteSpace(opts.LeftKeyColumn)) { throw new InvalidOperationException($"missing {nameof(LeftKeyColumn)}"); }
            if (string.IsNullOrWhiteSpace(opts.RightKeyColumn)) { throw new InvalidOperationException($"missing {nameof(RightKeyColumn)}"); }
            if (opts.Assertions == null || opts.Assertions.Count == 0) { throw new InvalidOperationException($"missing {nameof(Assertions)}"); }
            foreach (var assertion in opts.Assertions)
            {
                if (string.IsNullOrWhiteSpace(assertion.LeftColumnName)) { throw new InvalidOperationException($"missing {nameof(assertion.LeftColumnName)}"); }
                if (string.IsNullOrWhiteSpace(assertion.RightColumnName)) { throw new InvalidOperationException($"missing {nameof(assertion.RightColumnName)}"); }
            }

            return opts;
        }
    };
}
