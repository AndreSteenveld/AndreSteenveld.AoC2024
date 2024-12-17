using System.Text.RegularExpressions;
using AndreSteenveld.AoC;


var (left, right) = (new List<int>(), new List<int>());

var lines = Console.In.ReadLines().ToArray();
var regex = Numbers();

foreach (var line in lines) {
    var match = regex.Match(line);
    left.Add(Int32.Parse(match.Groups[1].Value));
    right.Add(Int32.Parse(match.Groups[2].Value));
}

var first_result = Enumerable
    .Zip(left.Order(), right.Order())
    .Select(t => Math.Abs(t.First - t.Second))
    .Sum();

var second_result = Enumerable.Sum(
    from l in left
    select l * right.Count(v => v == l)
);

Console.WriteLine($"First result :: {first_result}");
Console.WriteLine($"Second result :: {second_result}");

partial class Program
{
    [GeneratedRegex(@"^(\d+)\s+(\d+)$", RegexOptions.Compiled | RegexOptions.Singleline)]
    private static partial Regex Numbers();
}