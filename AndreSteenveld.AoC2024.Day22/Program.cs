using System.Collections;
using AndreSteenveld.AoC;

using NumberPriceDelta = (long current, long digit, long delta);

var seeds = Console.In.ReadLines().Select(Int64.Parse).ToArray();

var sum = seeds.Select(seed => RandomizeUsingSeed(seed).Skip(1999).First()).Sum();

Console.WriteLine($"Part 1 sum :: {sum}");


var (sequence, bananas) = seeds 
    .AsParallel()
    .SelectMany(seed => ValueSequences(seed).Take(2000 - 4).DistinctBy(sv => sv.sequence))
    .GroupBy(sv => sv.sequence, sv => sv.value)
    .Select(g => (sequence: g.Key, sum: g.Sum()))
    .MaxBy(sv => sv.sum);

Console.WriteLine($"Part 2 {sequence} :: {bananas}");

return;

IEnumerable<((long, long, long, long) sequence, long value)> ValueSequences(long seed) =>
    Sequences(seed)
        .Select((v, n1, n2, n3, n4) => (
            sequence: (n1.delta, n2.delta, n3.delta, n4.delta),
            value: v.digit + n1.delta + n2.delta + n3.delta + n4.delta
        ));
        
        // This is making the assumption that we're getting the first instance here, if this isn't the case this
        // is going to be doomed.
        // https://stackoverflow.com/questions/2475472/does-distinct-preserve-always-take-the-first-element-in-the-list
        // https://stackoverflow.com/questions/12428985/distinct-and-orderby-issue/12429107#12429107
        //.DistinctBy(sv => sv.sequence);

IEnumerable<(NumberPriceDelta, NumberPriceDelta, NumberPriceDelta, NumberPriceDelta, NumberPriceDelta)> Sequences(long seed) =>
    NumberPriceDelta(seed)
        .Zip(NumberPriceDelta(seed).Skip(1))
        .Zip(NumberPriceDelta(seed).Skip(2))
        .Zip(NumberPriceDelta(seed).Skip(3))
        .Zip(NumberPriceDelta(seed).Skip(4))
        .Select(t => (
            t.First.First.First.First,
            t.First.First.First.Second,
            t.First.First.Second,
            t.First.Second,
            t.Second
        ));

IEnumerable<NumberPriceDelta> NumberPriceDelta(long seed) => Enumerable
    .Zip(RandomizeUsingSeed(seed), RandomizeUsingSeed(seed).Skip(1))
    .Select((previous, current) => (current, current % 10, current % 10 - previous % 10));

IEnumerable<long> RandomizeUsingSeed(long secret) {

    yield return secret;
    
    while (true) {
        secret = (secret ^ (secret * 64)) % 16777216;
        secret = (secret ^ (secret / 32)) % 16777216;
        secret = (secret ^ (secret * 2048)) % 16777216;

        yield return secret;
    }
}

file static class _ {
    
    
}