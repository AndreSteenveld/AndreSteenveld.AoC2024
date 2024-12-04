
using System.Text.RegularExpressions;
using AndreSteenveld.AoC;

var (X, Y, field) = Console.In.ReadLines().Aggregate(
    (X:0, Y:0, field:""), (result, line) => (
        result.X switch {
            0 => line.Length + 1,
            var x when x == line.Length + 1 => x,
            _ => throw new Exception($"Jagged line [{result.Y + 1}] [{result.X} // {line.Length}]")
        },
        result.Y++,
        result.field + "\n" + line
    )
);

var xmax_lookers = new Dictionary<string, Regex>{
    ["East East"]   = EastEast(),
    ["South East"]  = new (@$"(X)(?=.{{{  X  }}}(M).{{{  X  }}}(A).{{{  X  }}}(S))", RegexOptions.Compiled | RegexOptions.Singleline),
    ["South South"] = new (@$"(X)(?=.{{{X - 1}}}(M).{{{X - 1}}}(A).{{{X - 1}}}(S))", RegexOptions.Compiled | RegexOptions.Singleline),
    ["South West"]  = new (@$"(X)(?=.{{{X - 2}}}(M).{{{X - 2}}}(A).{{{X - 2}}}(S))", RegexOptions.Compiled | RegexOptions.Singleline),
    ["West West"]   = WestWest(),
    ["North West"]  = new (@$"(S)(?=.{{{  X  }}}(A).{{{  X  }}}(M).{{{  X  }}}(X))", RegexOptions.Compiled | RegexOptions.Singleline),
    ["North North"] = new (@$"(S)(?=.{{{X - 1}}}(A).{{{X - 1}}}(M).{{{X - 1}}}(X))", RegexOptions.Compiled | RegexOptions.Singleline),
    ["North East"]  = new (@$"(S)(?=.{{{X - 2}}}(A).{{{X - 2}}}(M).{{{X - 2}}}(X))", RegexOptions.Compiled | RegexOptions.Singleline),
};

var x_lookers = new Dictionary<string, Regex> {
    ["MM"] = new (@$"(M)(?=[^\n](M).{{{X - 2}}}(A).{{{X - 2}}}(S)[^\n](S))", RegexOptions.Compiled | RegexOptions.Singleline),
    ["SM"] = new (@$"(S)(?=[^\n](M).{{{X - 2}}}(A).{{{X - 2}}}(S)[^\n](M))", RegexOptions.Compiled | RegexOptions.Singleline),
    ["SS"] = new (@$"(S)(?=[^\n](S).{{{X - 2}}}(A).{{{X - 2}}}(M)[^\n](M))", RegexOptions.Compiled | RegexOptions.Singleline),
    ["MS"] = new (@$"(M)(?=[^\n](S).{{{X - 2}}}(A).{{{X - 2}}}(M)[^\n](S))", RegexOptions.Compiled | RegexOptions.Singleline)
};

var found_xmas = xmax_lookers
    .AsParallel()
    .Select(kv => KeyValuePair.Create(kv.Key, kv.Value.Matches(field)))
    .ToDictionary();

var found_x = x_lookers
    .AsParallel()
    .Select(kv => KeyValuePair.Create(kv.Key, kv.Value.Matches(field)))
    .ToDictionary();

var number_of_xmas = found_xmas.Values.Select(mc => mc.Count).Sum();
var number_of_x = found_x.Values.Select(mc => mc.Count).Sum();

//Console.WriteLine(field);
//Console.WriteLine(show_matches(found.Values, field));
//Console.WriteLine(hide_matches(found.Values, field));

Console.WriteLine($"Total number of matches :: {number_of_xmas}");
Console.WriteLine($"Total of X found :: {number_of_x}");

return;

char[] hide_matches(IEnumerable<MatchCollection> collections, string field) {
    var indexs_in_match =
        from matches in collections
        from match in matches
        from @group in match.Groups.Values
        select @group.Index;

    return field.Select((c, i) => indexs_in_match.Contains(i) ? '.' : c).ToArray();
}

char[] show_matches(IEnumerable<MatchCollection> collections, string field) {
    var indexs_in_match =
        from matches in collections
        from match in matches
        from @group in match.Groups.Values
        select @group.Index;

    return field.Select((c, i) => c is '\n' || indexs_in_match.Contains(i) ? c : '.').ToArray();
}

partial class Program
{
    [GeneratedRegex("(X)(M)(A)(S)", RegexOptions.Compiled | RegexOptions.Singleline)]
    private static partial Regex EastEast();

    [GeneratedRegex("(S)(A)(M)(X)", RegexOptions.Compiled | RegexOptions.Singleline)]
    private static partial Regex WestWest();
}