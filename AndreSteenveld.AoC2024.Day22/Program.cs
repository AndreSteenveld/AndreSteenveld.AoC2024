using AndreSteenveld.AoC;

var seeds = Console.In.ReadLines().Select(Int64.Parse).ToArray();

var sum = seeds.Select(seed => RandomizeUsingSeed(seed).Skip(1999).First()).Sum();

Console.WriteLine($"Part 1 sum :: {sum}");

return;

IEnumerable<long> RandomizeUsingSeed(long secret) {
    
    while (true) {
        secret = (secret ^ (secret * 64)) % 16777216;
        secret = (secret ^ (secret / 32)) % 16777216;
        secret = (secret ^ (secret * 2048)) % 16777216;

        yield return secret;
    }

} 

file static class _ { }