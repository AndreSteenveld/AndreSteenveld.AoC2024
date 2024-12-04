using System.Text.RegularExpressions;
using AndreSteenveld.AoC;

var text = Console.In.ReadLines().ToString(preserveNewlines:false);

var summed_multipications = Enumerable.Sum(
    from match in MultiplicationCall().Matches(text)
    select Int32.Parse(match.Groups[1].Value) * Int32.Parse(match.Groups[2].Value)
);

Console.WriteLine($"Summed multiplications :: {summed_multipications}"); // 178538786

var summed_conditional_multiplications = Enumerable.Sum(
    from match in MultiplicationCall().Matches(RemoveDisabledSections().Replace(text, ""))
    select Int32.Parse(match.Groups[1].Value) * Int32.Parse(match.Groups[2].Value)
);

Console.WriteLine($"Summed conditional multiplications :: {summed_conditional_multiplications}"); // 102467299

partial class Program
{
    [GeneratedRegex(@"mul[(](\d{1,3})[,](\d{1,3})[)]")]
    private static partial Regex MultiplicationCall();
    
    [GeneratedRegex(@"(don't[(][)]).*?(do[(][)]|$)")]
    private static partial Regex RemoveDisabledSections();
}