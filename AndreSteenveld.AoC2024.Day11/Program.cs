using System.Diagnostics;
using AndreSteenveld.AoC;

using static System.Math;

var sw = new Stopwatch() ;; sw.Start();

ulong[] numbers = Console.In.ReadLines().Single().Split(' ').Select(UInt64.Parse).ToArray();

sw.Restart();

var number_of_stones_25 = numbers.Blink(25).Last().Length;

Console.WriteLine($"After blinking 25 times in [ {sw.ElapsedMilliseconds}ms ] the number of stones is :: {number_of_stones_25}") ;; sw.Restart();

var number_of_fast_stones_25 = numbers.BlinkReallyFast(25);

Console.WriteLine($"After blinking really fast 25 times in [ {sw.ElapsedMilliseconds}ms ] the number of stones is :: {number_of_fast_stones_25}") ;; sw.Restart();

var number_of_stones_75 = numbers.BlinkReallyFast(75);

Console.WriteLine($"After blinking really fast 75 times in [ {sw.ElapsedMilliseconds}ms ] the number of stones is :: {number_of_stones_75}") ;

return;

public static class _ {

    static int NumberOfDigits(ulong number) {
        for( var (number_of_digits, power) = (1, 1UL) ;; (number_of_digits, power) = (number_of_digits + 1, power * 10))
            if (power <= number && number < power * 10)
                return number_of_digits;
    }
        
    static ulong[] SplitNumber(ulong number) {
        for (ulong power = 10 ;; power *= 100)
            if (number >= power && number < power * 100) {
                var divider = (ulong)Pow(10, NumberOfDigits(power) / 2);
                return [number / divider, number % divider];
            }
    }

    static ulong[] Blink(ulong stone) =>
          stone is 0 ? [1UL]
        : NumberOfDigits(stone) % 2 is 0 ? SplitNumber(stone)
        : [stone * 2024]
        ;

     public static IEnumerable<ulong> Blink(this IEnumerable<ulong> line) =>
        line.SelectMany(Blink);

    public static Dictionary<ulong, ulong> BlinkMap(this ulong[] line) {
        var map = line.ToDictionary(stone => stone, Blink);

        for (var stones = UnblinkedStones(); stones.Length is not 0; stones = UnblinkedStones())
            foreach (var stone in stones)
                map[stone] = Blink(stone);

        return map.ToDictionary(kv => kv.Key, kv => (ulong)line.Count(kv.Key.Equals));
        
        ulong[] UnblinkedStones() {
            return map.Values.SelectMany(stone => stone).Distinct().Except(map.Keys).ToArray();
        }
    }
    
    public static ulong BlinkReallyFast(this ulong[] line, int times) {

        var map = BlinkMap(line);
        
        while(times --> 0) foreach (var (stone, occurrences) in map.ToList()) {
            
            if(occurrences is 0) continue;
            
            map[stone] -= occurrences;
            
            switch (Blink(stone)) {
                case [var s]:
                    map[s] += occurrences;
                    break;
                case [var left, var right]:
                    map[left] += occurrences;
                    map[right] += occurrences;
                    break;
            }

        }

        return map.Values.Aggregate(0UL, (sum, o) => sum + o);

    }

    public static IEnumerable<ulong[]> Blink(this IEnumerable<ulong> line, int times = 1) {
        yield return line.ToArray();
        
        if(times is 0)
            yield break;

        foreach (var l in line.Blink().Blink(times - 1))
            yield return l;
    }
    
    
}